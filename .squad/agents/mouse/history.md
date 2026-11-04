# Project Context

- **Owner:** Copilot
- **Project:** Generative AI for Beginners .NET — a hands-on course teaching .NET developers to build Generative AI applications with runnable samples.
- **Stack:** .NET 10+, Microsoft.Extensions.AI (MEAI), Microsoft Agent Framework (MAF), Azure OpenAI, Ollama, Aspire, Blazor
- **Created:** 2026-07-15

## Learnings

<!-- Append new learnings below. Each entry is something lasting about the project. -->
- No automated unit/integration test suite yet — validation is via CI build verification + manual sample runs.
- Required before commit: `dotnet format`, `dotnet build`, and `dotnet format --verify-no-changes` where linting is enabled.
- CI builds these solutions in Release: CoreSamples, Aspire.MCP.Sample, HFMCP.GenImage, SpaceAINet, MAF-Demos.

### 2026-08-22 — PR #526/#527/#528 validation investigation
- **Root cause of the recurring "Core Samples" CI failure (affects any PR that touches `samples/CoreSamples/**`, including #527 and #528, neither of which changed these files):** five projects target `net8.0-windows10.0.18362` with `RuntimeIdentifier=win-x64` and Windows-only packages (`Microsoft.AI.Foundry.Local.WinML`, `NAudio`, `ElBruno.Whisper`): `04-foundrylocal-native-chat-completions`, `05-foundrylocal-audio-transcription`, `06-foundrylocal-native-auto-chat`, `10-live-speech-to-text`, `11-foundrylocal-live-transcription`. `build-validation.yml` runs `dotnet build` on `ubuntu-latest`, which fails these 5 with NETSDK1100 ("set EnableWindowsTargeting to true") because there's no `global.json` pinning the SDK and no per-project `EnableWindowsTargeting` override. Confirmed by: identical build succeeding on Windows locally, and the same 5-project failure pattern reproducing identically on both #527 and #528 (which touch zero shared files). This is a pre-existing/environmental gap on `main`, not a PR regression — any new Core Samples PR will hit it until fixed (e.g. add `<EnableWindowsTargeting>true</EnableWindowsTargeting>` to those 5 csproj files, or move/split that CI job to `windows-latest`).
- `dotnet format samples/CoreSamples/CoreGenerativeAITechniques.sln --verify-no-changes` currently fails on `main` itself (124 whitespace errors across 9 files, worst offender `Common/FoundryLocalSampleSupport.cs`), unrelated to any of the three open PRs. `build-validation.yml` does not run a format-check job today, so this debt isn't CI-gated yet — flag before adding a format gate.
- Public GitHub REST endpoints (`/commits/{sha}/check-runs`, `/check-runs/{id}/annotations`, `/pulls/{n}`, `/repos/.../tags`) work **unauthenticated** for this public repo and were sufficient to diagnose CI failures precisely (annotation text alone identified NETSDK1100). Authenticated calls (`gh api .../actions/jobs/{id}/logs`, GraphQL via `gh pr view`) are blocked by Microsoft org SAML enforcement for the current token — don't waste time retrying those; annotations are usually enough.
- Non-destructive investigation pattern that works well here: `git fetch origin pull/<n>/head:refs/heads/pr-<n>-check` + `git worktree add <sibling-dir> pr-<n>-check`, validate, then `git worktree remove --force` and `git branch -D` to clean up. Never checkout PR heads onto the shared `main` working tree.
- The shared file-based-app secrets store lives at `%APPDATA%\Microsoft\UserSecrets\genai-beginners-dotnet\secrets.json` (the `UserSecretsId` convention used by shared samples) and is excluded from tool access by org content policy — don't attempt to read/copy/back it up even for legitimate before/after testing; treat it as untouchable and test around it instead (e.g. use an Ollama-only sample to exercise a shared library code path instead of toggling a shared Foundry secret).
- `PR #526` (`AzFunction-BasicChat-01`) establishes a good template for future Azure Function samples: mock-based xUnit tests (`IChatResponder` fake, no live Azure calls needed in CI), `IOptions<T>` + `ValidateDataAnnotations().ValidateOnStart()` for config, keyless `DefaultAzureCredential`, and logs that never include prompt/answer text. Worth reusing for the next Azure Function sample in this repo.
