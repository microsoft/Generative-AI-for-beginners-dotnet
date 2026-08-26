# AGENTS.md

## Project Overview

This repository contains **Generative AI for Beginners .NET** - a hands-on course teaching .NET developers how to build Generative AI applications. The course focuses on practical, runnable code samples and real-world applications.

### Architecture

- **Lesson-based structure**: Organized in numbered folders (01-05) containing lesson documentation
- **Centralized samples**: All code samples live in the `samples/` directory, organized by category
- **Multi-language support**: Course materials translated into 8 languages (located in `translations/`)
- **Sample applications**: Full-featured AI-powered apps demonstrating practical GenAI integration
- **Key technologies**: .NET 10+, Microsoft.Extensions.AI (MEAI), Microsoft Agent Framework, Azure OpenAI, Ollama

### Project Structure

```
.
├── 01-IntroductionToGenerativeAI/     # Introduction to Generative AI + setup guides
├── 02-GenerativeAITechniques/         # Text completions, streaming, function calling, middleware
├── 03-AIPatternsAndApplications/      # Embeddings, RAG, vision, combining patterns
├── 04-AgentsWithMAF/                  # Microsoft Agent Framework + MCP
├── 05-ResponsibleAI/                  # Responsible AI practices
├── samples/
│   ├── CoreSamples/                   # Core technique samples (chat, vision, RAG, functions, etc.)
│   │   └── CoreGenerativeAITechniques.sln
│   ├── MAF/                           # Microsoft Agent Framework samples (25+ projects)
│   │   └── MAF-Demos.slnx
│   ├── AppsWithGenAI/                 # Full AI-powered applications
│   │   ├── SpaceAINet/               # AI-powered Space Battle game
│   │   ├── HFMCP.GenImage/           # Image generation Aspire app
│   │   └── ConsoleGpuViewer/         # GPU viewer console app
│   └── PracticalSamples/             # Aspire MCP sample
│       └── Aspire.MCP.Sample.sln
├── infra/                             # Azure Bicep infrastructure
├── translations/                      # Localized course materials (8 languages)
└── images/                            # Course branding assets

Each lesson folder contains:
- readme.md: Lesson documentation with step-by-step guidance
- images/: Visual assets
- Setup guides (in lesson 01)
```

## Setup Commands

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) or later
- Access to at least one AI provider:
    - **Azure OpenAI** (requires Azure subscription)
  - **Ollama** (for local model execution)

### Initial Setup

```bash
# Clone the repository
git clone https://github.com/microsoft/Generative-AI-for-beginners-dotnet.git
cd Generative-AI-for-beginners-dotnet

# Verify .NET installation
dotnet --version  # Should be 10.0 or higher
```

### AI Provider Setup

#### Azure OpenAI
```bash
# Navigate to a project and set user secrets
cd <project-path>
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "<your-endpoint>"
dotnet user-secrets set "AZURE_OPENAI_MODEL" "<your-model-name>"
dotnet user-secrets set "AZURE_OPENAI_APIKEY" "<your-api-key>"
```

#### Ollama (Local Models)
```bash
# Install Ollama from https://ollama.com/download

# Pull required models (example for lesson 02)
ollama pull phi4-mini

# Pull models for RAG samples (lesson 03)
ollama pull phi4-mini
ollama pull all-minilm

# Verify Ollama is running (default: http://localhost:11434)
curl http://localhost:11434/api/version
```

### Installing Dependencies

```bash
# Restore dependencies for a specific solution
dotnet restore <path-to-solution>.sln

# Example: Restore core samples
dotnet restore samples/CoreSamples/CoreGenerativeAITechniques.sln

# Restore MAF samples
dotnet restore samples/MAF/MAF-Demos.slnx

# Restore Aspire MCP sample
dotnet restore samples/PracticalSamples/Aspire.MCP.Sample.sln
```

## Development Workflow

### GitHub Codespaces (Recommended)

1. Fork this repository to your GitHub account
2. Create a new Codespace from your fork
3. Select the appropriate dev container:
   - Default: .NET 10 with Azure CLI and GitHub CLI
   - Ollama: Includes Ollama for local model execution
4. The postCreateCommand will automatically set up .NET workloads and trust dev certificates

### Local Development

```bash
# Start a sample project
cd <project-directory>
dotnet run

# Example: Run basic chat sample with MEAI
cd samples/CoreSamples/BasicChat-01MEAI
dotnet run app.cs

# Example: Run with Ollama
cd samples/CoreSamples/BasicChat-03Ollama
dotnet run
```

### Working with Sample Applications

#### SpaceAINet (AI-powered game)
```bash
cd samples/AppsWithGenAI/SpaceAINet/SpaceAINet.Console
dotnet build
dotnet run

# In-game controls:
# A - Toggle Azure OpenAI mode
# O - Toggle Ollama mode
# F - Show FPS
# S - Save screenshot
```

#### Aspire MCP Sample
```bash
cd samples/PracticalSamples/src
dotnet run --project McpSample.AppHost/McpSample.AppHost.csproj
```

### Hot Reload and Watch Mode

```bash
# Run with hot reload (for web projects)
dotnet watch run

# Build in watch mode
dotnet watch build
```

## Testing Instructions

### Build Validation

The repository uses GitHub Actions for CI/CD validation. Before submitting changes:

```bash
# Restore dependencies
dotnet restore <solution-path>

# Build in Release mode (as CI does)
dotnet build <solution-path> --no-restore --configuration Release --verbosity minimal

# Example: Validate core samples
dotnet restore samples/CoreSamples/CoreGenerativeAITechniques.sln
dotnet build samples/CoreSamples/CoreGenerativeAITechniques.sln --no-restore --configuration Release
```

### Validated Solutions

The following solutions are validated in CI:
- `samples/CoreSamples/CoreGenerativeAITechniques.sln`
- `samples/PracticalSamples/Aspire.MCP.Sample.sln`
- `samples/AppsWithGenAI/HFMCP.GenImage/HFMCP.GenImage.sln`
- `samples/AppsWithGenAI/SpaceAINet/SpaceAINet.sln`
- `samples/MAF/MAF-Demos.slnx`

### Validated File-Based `app.cs` Samples

In addition to the solution matrix above, `.github/workflows/build-validation.yml` runs a
secret-free `discover-file-based-apps` / `compile-file-based-apps` job pair that:
- Discovers every tracked `samples/**/app.cs` file with `git ls-files` (no manual list to keep in sync).
- Compiles each one independently with `dotnet build <path>` (no run, no credentials, no Ollama).
- Uses `fail-fast: false` so every sample reports its own pass/fail, named by its path.

To intentionally exclude a sample from this check (for example, a future OS-specific or
credential-only file-based app), add its exact `git ls-files` path to the `EXCLUDED_SAMPLES`
array in the `Discover tracked app.cs samples` step, with a comment explaining why. This list is
empty today — every tracked file-based sample must compile secret-free.

### Ollama Smoke Validation (opt-in, `workflow_dispatch` only)

Compilation alone cannot catch runtime-only regressions in the OllamaSharp integration (see
the serialization bug fixed by #528, which only surfaced on the first live model call).
`.github/workflows/ollama-smoke-validation.yml` closes that gap with a **manually triggered**
workflow — it has no `push`/`pull_request` trigger, so it never runs automatically and never
adds a model download to normal PR validation. Trigger it from the Actions tab
(**Ollama Smoke Validation (opt-in)** → **Run workflow**) or with
`gh workflow run ollama-smoke-validation.yml`.

**Sample scope** — exactly three file-based samples are exercised, each covering a distinct
OllamaSharp code path:
- `samples/CoreSamples/BasicChat-03Ollama/app.cs` — single-turn request path, run exactly as
  committed.
- `samples/CoreSamples/BasicChat-10ConversationHistory/app.cs` — run with no Azure user-secrets
  configured, so the sample's own fallback logic (already in the committed source) selects the
  local Ollama client. Two questions are piped over stdin to exercise the streaming
  (`GetStreamingResponseAsync`) and multi-turn conversation-history paths in one run.
- `samples/CoreSamples/BasicChat-07Ollama-gpt-oss/app.cs` — **the tracked file is never
  modified.** The workflow copies it to a runner-local `$RUNNER_TEMP` directory and only there
  substitutes the model tag before compiling/running the copy (see Substitution policy below).
  The workflow step verifies with `git diff --quiet` that the tracked source is untouched.

**Substitution policy & model-tag drift risk** — the workflow pulls a single small pinned
model, `phi4-mini` (overridable via the `model` `workflow_dispatch` input for maintainers who
need to test a different tag). `BasicChat-07Ollama-gpt-oss` is written against `gpt-oss:20b`,
a much larger reasoning model that isn't practical to pull on a hosted runner within a bounded
timeout. Swapping in `phi4-mini` only proves the OllamaSharp request path (client
construction, serialization, response parsing) works end-to-end — it does **not** validate
`gpt-oss:20b`-specific reasoning quality or behavior. If OllamaSharp ever regresses in a way
that's specific to larger/reasoning models, this substitution would not catch it; treat a green
run here as evidence for the shared client/serialization path only, not as a claim about
`gpt-oss:20b` itself.

**Timeout behavior** — every step is individually bounded so a hang fails fast with a clear
step name instead of consuming the whole job budget: readiness polling is capped at 60s
(30 attempts × 2s), the model pull at 900s, and each sample run at 180s via `timeout`. The job
itself has a 30-minute `timeout-minutes` ceiling. Step names and `GITHUB_STEP_SUMMARY` entries
distinguish Ollama install/readiness/model-pull failures from sample-level regressions, and the
Ollama daemon is stopped best-effort in an `if: always()` cleanup step regardless of outcome.

**Assertions** — a sample only passes if it exits 0 **and** produces a model response of
meaningful length (≥15 words) with no exception/serialization-error signatures in its output;
for `BasicChat-10ConversationHistory` the check additionally confirms the Ollama fallback label
was printed (proving no Azure secrets were picked up), that two streamed answers were produced,
and that the second answer references the first question's topic (proving conversation history
was actually carried across turns, not just that the process exited cleanly).

No repository/org/cloud secrets are used anywhere in this workflow, and nothing secret is ever
printed.

### Manual Testing

```bash
# Test a specific project
cd <project-directory>
dotnet run

# Verify AI integration works with your provider
# Follow the lesson-specific README instructions for testing AI features
```

### Note on Test Coverage

This repository is primarily educational with sample applications. There are currently no automated unit or integration tests. Validation is done through:
- Build verification in CI
- Manual testing of sample applications
- Documentation review

## Code Style and Conventions

### Formatting

```bash
# Format code before committing (REQUIRED)
dotnet format

# Verify formatting without making changes
dotnet format --verify-no-changes

# Format a specific solution
dotnet format <path-to-solution>.sln
```

### C# Conventions

- Use modern .NET features (top-level statements, nullable reference types, file-scoped namespaces)
- Follow standard C# naming conventions (PascalCase for classes/methods, camelCase for locals)
- Use async/await patterns for I/O operations
- Prefer dependency injection over static instances
- Use `IChatClient` abstraction (Microsoft.Extensions.AI) for AI model integration

### AI Integration Patterns

- **Never hardcode API keys** - use user secrets or environment variables
- **Abstract AI providers** behind service classes for easy swapping
- **Use IChatClient** from Microsoft.Extensions.AI for provider-agnostic code
- **Microsoft Agent Framework** for advanced agent orchestration and multi-agent workflows
- Support multiple providers (Azure OpenAI, Ollama) where applicable

### File Organization

- Place sample code in the `samples/` directory under the appropriate category
- Store images in `<lesson-number>/images/` or root `images/` folder
- Keep README files in lesson root directories
- Use descriptive file names with English characters, numbers, and dashes

### Documentation Standards

- All URLs must be wrapped in `[text](url)` format
- Relative links start with `./` (current dir) or `../` (parent dir)
- Add tracking IDs to Microsoft/GitHub URLs: `?wt.mc_id=` or `&wt.mc_id=`
- No country-specific locales in URLs (avoid `/en-us/`)
- Update relevant lesson README.md files when modifying code

## Build and Deployment

### Build Commands

```bash
# Standard build
dotnet build

# Release build (as used in CI)
dotnet build --configuration Release

# Build specific solution
dotnet build <path-to-solution>.sln
```

### .NET Workloads

The dev container automatically installs required workloads. For local development:

```bash
# Update workloads
dotnet workload update

# List installed workloads
dotnet workload list

# Trust development certificates
dotnet dev-certs https --trust
```

### Deployment Notes

- Sample projects are designed for local execution and learning
- For production deployment, follow Azure deployment best practices
- Aspire projects can be deployed to Azure Container Apps
- Refer to individual project READMEs for specific deployment instructions

## Pull Request Guidelines

### Before Submitting a PR

1. **Format your code**: Run `dotnet format` on all modified projects
2. **Build verification**: Ensure `dotnet build` succeeds for affected solutions
3. **Test manually**: Run and verify sample applications work as expected
4. **Update documentation**: Modify lesson READMEs if changing functionality
5. **Translation considerations**: For content changes, create separate PRs per language

### PR Requirements

- **Fork the repository** before making changes
- **One change per PR**: Separate bug fixes, features, and documentation updates
- **No partial translations**: Submit complete translations for all files in a language
- **Keep main branch updated**: Rebase if there are merge conflicts
- **CLA required**: Contributors must agree to Microsoft's Contributor License Agreement

### Commit Message Format

```
<type>: <brief description>

Examples:
- docs: Update Azure OpenAI setup instructions
- feat: Add new RAG sample with vector search
- fix: Correct null reference in chat service
- style: Apply dotnet format to all projects
```

### PR Title Format

Use descriptive titles that clearly indicate the change:
- `docs: Update lesson 02 for .NET 10 compatibility`
- `feat: Add Agent Framework orchestration sample`
- `fix: Resolve build errors in SpaceAINet project`

## Additional Notes

### Common Issues and Solutions

#### Build Errors
- **Missing SDK**: Ensure .NET 10 SDK is installed (`dotnet --version`)
- **Workload issues**: Run `dotnet workload update`
- **Package restore fails**: Delete `bin/` and `obj/` folders, then `dotnet restore`

#### AI Provider Issues
- **Azure OpenAI**: Check user secrets are configured: `dotnet user-secrets list`
- **Ollama**: Ensure service is running and model is pulled: `ollama list`

#### Codespace-Specific
- **Port forwarding**: Ensure port 17057 is forwarded for web applications
- **Memory limits**: Codespace requires 16GB RAM for optimal performance
- **Ollama in Codespaces**: Use the Ollama dev container configuration

### Security Considerations

- **Never commit secrets** to source control
- **Use user secrets** for local development: `dotnet user-secrets set`
- **Environment variables** for production/Codespaces
- **Review .gitignore** before committing to exclude sensitive files

### Performance Tips

- **Local models (Ollama)**: Require significant CPU/GPU resources
- **Cloud models**: Faster but require network connectivity
- **Batch operations**: Use streaming responses for better UX
- **Caching**: Consider caching AI responses for repeated queries

### Multi-Language Support

Translations are maintained in `translations/<language-code>/`:
- Only contribute translations in languages where you are proficient
- Do not use machine translation
- Submit complete translations (all files) in a single PR
- Update the translation table in README.md with the date

### Project-Specific Context

#### Lesson 01: Introduction to Generative AI
- What generative AI is, why .NET is a first-class citizen for AI
- Setup guides for Azure OpenAI and Ollama
- GitHub Codespaces configuration

#### Lesson 02: Generative AI Techniques
- Text completions, chat conversations, streaming, structured output
- Function calling and middleware pipeline
- Core samples: BasicChat-01MEAI, BasicChat-03Ollama, MEAIFunctions

#### Lesson 03: AI Patterns and Applications
- Embeddings and semantic search
- RAG (Retrieval-Augmented Generation) with vectors
- Vision and document understanding
- Combining AI patterns, local model runners

#### Lesson 04: Agents with MAF
- Building first agent with Microsoft Agent Framework
- Agents with tools, multi-agent workflows
- Model Context Protocol (MCP) integration
- 25+ MAF sample projects

#### Lesson 05: Responsible AI
- Identifying and mitigating bias
- Content safety, guardrails, transparency
- Ethical considerations for agentic systems

#### Sample Applications (samples/AppsWithGenAI/)
- **SpaceAINet**: AI-powered retro Space Battle game (created with Copilot Agent)
- **HFMCP.GenImage**: Image generation Aspire app with HuggingFace MCP
- **ConsoleGpuViewer**: GPU monitoring console app

#### Practical Samples (samples/PracticalSamples/)
- Aspire MCP sample demonstrating Model Context Protocol
- Multi-project Aspire solution with Blazor chat UI
- Integration with external MCP servers

### Contributing to Documentation

When updating documentation:
1. Maintain consistent formatting across all READMEs
2. Include code examples where helpful for beginners
3. Add links to official documentation for deeper learning
4. Keep explanations accessible to .NET developers new to AI

### Resource Links

- [.NET Documentation](https://learn.microsoft.com/dotnet/)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions)
- [Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/)
- [Azure OpenAI](https://learn.microsoft.com/azure/ai-services/openai/)
- [Ollama Documentation](https://ollama.com/docs)

---

**For more information**, refer to:
- [README.md](./README.md) - Course overview and lesson map
- [CONTRIBUTING.MD](./CONTRIBUTING.MD) - Detailed contribution guidelines
- Individual lesson READMEs for specific topics and samples
