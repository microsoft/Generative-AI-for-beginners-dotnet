"""Foundry Local docs/offline checks, builds, and explicit cached-model inference.

No third-party Python dependencies. Model downloads require --prepare.
"""

import argparse
import json
import os
from pathlib import Path
import re
import tempfile
import sys

from test_ollama import execute, relative_link_errors, require

ROOT = Path(__file__).resolve().parents[1]
CORE = ROOT / "samples" / "CoreSamples"
PROJECTS = [
    CORE / "FoundryLocal-00NativeChat",
    CORE / "FoundryLocal-01Techniques",
    CORE / "FoundryLocal-02RAG",
    ROOT / "samples" / "MAF" / "MAF-FoundryLocal-01",
    CORE / "MCP-FoundryLocal-LocalTools",
]
TESTS = CORE / "FoundryLocal.Tests" / "FoundryLocal.Tests.csproj"
SETUP = ROOT / "01-IntroductionToGenerativeAI" / "setup-foundry-local.md"
DOCS = [
    ROOT / "README.md", SETUP, ROOT / "docs" / "foundry-local-learning-path.md",
    CORE / "FOUNDRY-LOCAL-SAMPLES-README.md",
    ROOT / "03-AIPatternsAndApplications" / "05-LocalModelRunners.md",
    *[ROOT / lesson / "readme.md" for lesson in (
        "01-IntroductionToGenerativeAI", "02-GenerativeAITechniques",
        "03-AIPatternsAndApplications", "04-AgentsWithMAF", "05-ResponsibleAI",
    )],
    *[project / "README.md" for project in PROJECTS],
    *[path for lesson in ("02-GenerativeAITechniques", "03-AIPatternsAndApplications", "04-AgentsWithMAF")
      for path in (ROOT / lesson).glob("*.md")],
]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--build-only", action="store_true")
    parser.add_argument("--prepare", action="store_true", help="Explicitly allow chat/embedding model downloads")
    parser.add_argument("--run", action="store_true", help="Run all core inference checks; never download models")
    parser.add_argument("--timeout", type=int, default=300)
    parser.add_argument("--sdk-version")
    parser.add_argument("--log-dir", type=Path)
    args = parser.parse_args()
    require(args.timeout > 0, "Timeout must be positive.")
    if args.sdk_version:
        require(re.fullmatch(r"\d+\.\d+\.\d+(?:-[\w.-]+)?", args.sdk_version), "Invalid SDK version.")
    if args.log_dir:
        args.log_dir.mkdir(parents=True, exist_ok=True)
    passed = 0

    def check(name, command, env, cwd, pattern=None, success=True, timeout=None):
        nonlocal passed
        code, output = execute(command, env, cwd=cwd, timeout=timeout or args.timeout, expect_success=False)
        if args.log_dir:
            (args.log_dir / (re.sub(r"[^a-zA-Z0-9-]", "-", name) + ".log")).write_text(output, encoding="utf-8")
        require((code == 0) == success, f"{name}: unexpected exit {code}\n{output}")
        if pattern:
            require(re.search(pattern, output, re.I | re.S), f"{name}: missing expected evidence\n{output}")
        passed += 1
        print(f"PASS {name}", flush=True)

    with tempfile.TemporaryDirectory(prefix="foundry-course-check-") as directory:
        work = Path(directory)
        if args.sdk_version:
            (work / "global.json").write_text(json.dumps({
                "sdk": {"version": args.sdk_version, "rollForward": "disable"},
            }), encoding="utf-8")
        env = {
            key: value for key, value in os.environ.items()
            if not key.upper().startswith(("AZURE", "OPENAI", "HF_", "OLLAMA_"))
        }
        env.update({"DOTNET_CLI_TELEMETRY_OPTOUT": "1", "ORT_TELEMETRY_DISABLED": "1"})
        errors = []
        for doc in DOCS:
            require(doc.is_file(), f"Missing documentation: {doc}")
            errors.extend(relative_link_errors(doc, doc.read_text(encoding="utf-8-sig")))
        require(not errors, "\n".join(errors))
        passed += 1
        print("PASS route documentation links and anchors", flush=True)
        check("offline-contract-tests", ["dotnet", "test", str(TESTS), "-c", "Release", "--nologo", "-v:minimal",
                                         "-p:NuGetAuditMode=all", "-warnaserror:NU1901,NU1902,NU1903,NU1904"],
              env, work, r"Failed:\s+0.*Passed:\s+\d+")
        if args.build_only or args.prepare or args.run:
            for project in PROJECTS:
                check("build-" + project.name,
                      ["dotnet", "build", str(project), "-c", "Release", "--nologo", "-v:q",
                       "-p:NuGetAuditMode=all", "-warnaserror:NU1901,NU1902,NU1903,NU1904"], env, work)
            snippets = re.findall(r"```csharp\s*\n(.*?)```", SETUP.read_text(encoding="utf-8"), re.S)
            require(len(snippets) == 1, "Keep setup snippet compilation in sync with documentation.")
            snippet = work / "snippet"
            snippet.mkdir()
            props = CORE / "Common" / "FoundryLocal.Course.props"
            (snippet / "Snippet.csproj").write_text(
                f'<Project Sdk="Microsoft.NET.Sdk"><Import Project="{props.as_posix()}" />'
                '<PropertyGroup><OutputType>Exe</OutputType></PropertyGroup></Project>', encoding="utf-8")
            (snippet / "Program.cs").write_text(snippets[0], encoding="utf-8")
            check("compile-setup-snippet", ["dotnet", "build", str(snippet), "-v:q"], env, work)

            def sample(index, mode=None):
                project = PROJECTS[index]
                matches = list((project / "bin" / "Release" / "net10.0").glob(f"*/{project.name}.dll"))
                require(len(matches) == 1, f"Expected exactly one current host RID output for {project.name}: {matches}")
                return ["dotnet", str(matches[0]), *([mode] if mode else [])]

            check("local-MCP-protocol", sample(4, "protocol"), env, work,
                  r"Local MCP protocol round trip complete.*Local MCP GetLibraryHours invoked")
            if args.prepare:
                check("prepare-chat-and-embeddings", [*sample(2, "prepare"), "--prepare"], env, work,
                      r"Chat and embedding models prepared", timeout=max(args.timeout, 1800))
            if args.run:
                cases = [
                    ("native-hello", 0, None, r"Native answer:\s*4"),
                    ("MEAI-hello", 1, "hello", r"Answer:\s*4"),
                    ("history", 1, "history", r"Remembered:.*ORCHID"),
                    ("streaming", 1, "stream", r"Streaming text chunks:\s*[1-9]\d*"),
                    ("structured", 1, "structured", r'\{"Sentiment":"Positive","Rating":5\}'),
                    ("cache", 1, "cache", r"Model calls:\s*1"),
                    ("functions", 1, "tools", r"Math without tools:.*tool calls: 0.*Library tool calls:\s*1"),
                    ("RAG-answer", 2, "answer", r"Embedding dimensions:\s*[1-9]\d*.*Retrieved: library.*Answer:.*\[library\]"),
                    ("RAG-unknown", 2, "unknown", r"Answer:.*don't know"),
                    ("RAG-injection", 2, "injection", r"Answer:.*\[library\]"),
                    ("agent", 3, "story", r"Writer:\s*\S"),
                    ("agent-tools", 3, "tools", r"Agent GetLibraryHours invoked.*Agent tool calls:\s*1"),
                    ("sequential-agents", 3, "workflow", r"Writer:\s*\S.*Editor:\s*\S"),
                    ("MCP-inference", 4, None, r"MCP tool calls:\s*1.*Local MCP GetLibraryHours invoked"),
                ]
                for name, index, mode, pattern in cases:
                    check(name, sample(index, mode), env, work, pattern)
                for variable, value, pattern in (
                    ("FOUNDRY_LOCAL_MODEL", "", r"must not be empty"),
                    ("FOUNDRY_LOCAL_MODEL", "missing-course-model-123456", r"not found|unknown|not.*available"),
                    ("FOUNDRY_LOCAL_MODEL", "qwen3-embedding-0.6b", r"Wrong model task"),
                    ("FOUNDRY_LOCAL_EMBEDDING_MODEL", "phi-4-mini", r"Wrong model task"),
                ):
                    negative = dict(env)
                    negative[variable] = value
                    check("reject-" + variable + "-" + (value or "empty"),
                          sample(2 if "EMBEDDING" in variable else 1), negative, work, pattern, success=False)
                check("invalid-mode", sample(1, "invalid"), env, work, r"Usage:", success=False)
        print(f"Summary: {passed} passed; 0 failed; 0 skipped. "
              + ("Core inference executed." if args.run else "Core inference NOT executed."), flush=True)


if __name__ == "__main__":
    try:
        main()
    except (AssertionError, TimeoutError) as error:
        print(f"FAIL {error}\nSummary: validation incomplete; at least 1 failed. No missing core check counts as PASS.", file=sys.stderr)
        sys.exit(1)
