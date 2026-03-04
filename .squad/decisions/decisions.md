# Team Decisions Log

This document consolidates architectural, implementation, and process decisions made by the .squad/ team.

---

## 1. Azure OpenAI Auth Migration to AzureCliCredential

**Author:** Neo  
**Date:** 2026-03-04  
**Status:** Implemented  
**Scope:** 8 file-based app.cs samples

### Decision

Migrate file-based .NET 10 app.cs samples from `ApiKeyCredential` to `AzureCliCredential` for Azure OpenAI authentication. Fix config key mismatch where samples read `config["endpoint"]` and `config["apikey"]` but user secrets are set as `config["AzureOpenAI:Endpoint"]` and `config["AzureOpenAI:Deployment"]`.

### Rationale

1. **Modern auth pattern:** `AzureCliCredential()` is the modern Azure SDK auth pattern — no API key needed, just `az login`. Aligns with Azure best practices and simplifies local development.
2. **Config key mismatch:** File-based apps had hardcoded wrong keys (`endpoint`, `apikey`) but `setup.ps1` writes secrets as `AzureOpenAI:Endpoint`, `AzureOpenAI:Deployment`, etc.
3. **Reference pattern:** https://github.com/Azure-Samples/agent-skills-dotnet-demo uses `AzureCliCredential()` pattern successfully for file-based .NET 10 apps.
4. **Security:** Eliminates API key storage in user secrets for standard Azure OpenAI samples.

### Changes Made

#### GROUP 1: AzureCliCredential migration (5 files)
- **Files:** BasicChat-01MEAI, BasicChat-05AIFoundryModels, MEAIFunctions, MEAIFunctionsAzureOpenAI, ImageGeneration-01
- **Changes:**
  - Added `#:package Azure.Identity@1.18.0` directive
  - Added `using Azure.Identity;`
  - Removed `using System.ClientModel;` (no longer needed)
  - Changed `config["endpoint"]` → `config["AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException("Set AzureOpenAI:Endpoint in User Secrets. See: https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/01-IntroductionToGenerativeAI/setup-azure-openai.md")`
  - Removed `var apiKey = new ApiKeyCredential(config["apikey"]);`
  - Changed `new AzureOpenAIClient(new Uri(endpoint), apiKey)` → `new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())`
  - Kept `?? "gpt-5-mini"` fallback on deployment name

#### GROUP 2: Config key fixes only (3 files)
- **BasicChat-08LLMTornado:**
  - Changed `config["endpoint"]` → `config["AzureOpenAI:Endpoint"]`
  - Changed `config["apikey"]` → `config["AzureOpenAI:ApiKey"]`
  - Added `?? throw` null checks with error messages
  - **Kept ApiKeyCredential:** LlmTornado library requires API key string, cannot use AzureCliCredential

- **BasicChat-06OpenAIAPIs:**
  - Changed `config["APIKEY"]` → `config["OpenAI:ApiKey"]`
  - Added `?? throw` null check
  - **Not Azure OpenAI:** Uses OpenAI direct API, different auth pattern

- **BasicChat-11FoundryClaude:**
  - Changed `config["endpoint"]` → `config["AzureOpenAI:Endpoint"]`
  - Changed `config["apikey"]` → `config["AzureOpenAI:ApiKey"]`
  - Changed `config["endpointClaude"]` → `config["Claude:Endpoint"]`
  - Changed `config["deploymentName"]` → `config["Claude:Deployment"]`
  - Added `?? throw` null checks with error messages
  - **Kept ApiKeyCredential:** Claude adapter requires API key for `x-api-key` header in custom HttpMessageHandler

#### GROUP 3: No changes needed (5 files)
- BasicChat-03Ollama, BasicChat-07Ollama-gpt-oss, BasicChat-09LLMTornadoOllama, BasicChat-10ConversationHistory (Ollama only)
- AgentLabs-01-Simple (already uses DefaultAzureCredential)

### Implications

1. **Users must run `az login`** before running GROUP 1 samples. This is documented in the setup guide.
2. **LlmTornado and Claude samples still require API key** in user secrets — documented exceptions. Will update `setup-secrets.ps1` to include `AzureOpenAI:ApiKey` for these special cases.
3. **Future file-based samples** using Azure OpenAI should use `AzureCliCredential()` pattern unless library requires API key string.
4. **Config key convention:** All Azure OpenAI config keys must be namespaced (`AzureOpenAI:*`, `Claude:*`, `OpenAI:*`).

### Team Impact

- **Doc team:** Update setup guide and lesson docs to mention `az login` prerequisite
- **Trinity:** No translation changes needed (code-only update)
- **Future contributors:** Follow AzureCliCredential pattern in CONTRIBUTING.MD

---

## 2. File-Based Sample Documentation Standard

**Author:** Trinity  
**Date:** 2026-03-04  
**Status:** Implemented  

### Context

The repository contains 13 .NET 10 file-based samples (CoreSamples with `app.cs`, no `.csproj`). These samples require `dotnet run app.cs` syntax, not `dotnet run`. Additionally, Azure OpenAI samples now use `AzureCliCredential()` for authentication, requiring users to run `az login` before execution.

Documentation was inconsistent:
- Some setup docs said `dotnet run` (incorrect for file-based samples)
- Azure authentication requirements (`az login`) were not prominently mentioned
- Users following incorrect commands experienced setup failures

### Decision

All documentation referencing file-based sample execution must:

1. **Syntax:** Use `dotnet run app.cs` (not `dotnet run`) for file-based samples
2. **Authentication:** Include a clear note that Azure OpenAI samples require `az login` before running
3. **Scope:** Changes apply to root docs (README, setup scripts) and lesson docs (setup guides)

### Implementation

**7 files updated:**
- `README.md` — Root setup section + `az login` note
- `01-IntroductionToGenerativeAI/setup-azure-openai.md` — 3 instances fixed + removed ambiguity
- `setup.ps1` — Setup completion message
- `setup-secrets.ps1` — Completion message clarity
- `02-GenerativeAITechniques/01-text-completions-chat.md` — Sample execution section
- `samples/CoreSamples/BasicChat-10ConversationHistory/README.md` — Sample execution
- `samples/CoreSamples/BasicChat-11FoundryClaude/README.md` — Sample execution

**13 file-based samples affected:**
- BasicChat-01MEAI, BasicChat-03Ollama, BasicChat-05AIFoundryModels, BasicChat-06OpenAIAPIs
- BasicChat-07Ollama-gpt-oss, BasicChat-08LLMTornado, BasicChat-09LLMTornadoOllama
- BasicChat-10ConversationHistory, BasicChat-11FoundryClaude, ImageGeneration-01
- MEAIFunctions, MEAIFunctionsAzureOpenAI, AgentLabs-01-Simple

**No changes to:**
- MAF and AppsWithGenAI samples (use traditional `.csproj` projects with `dotnet run`)
- Translation files (will be updated independently by translators using English docs as reference)

### Rationale

1. **Beginner experience:** Incorrect commands cause immediate setup failures and frustration
2. **Accuracy:** .NET 10 file-based samples require explicit filename in the `dotnet run` command
3. **Azure auth clarity:** `az login` is the current auth method (AzureCliCredential) and must be documented prominently
4. **Consistency:** All setup paths (automated, manual, verification) must use same syntax

### Impact

- Students will successfully run file-based samples on first try
- Setup docs become the single source of truth (no ambiguity between samples)
- Future contributors will follow established pattern when adding file-based samples

### Future Scope

- Translations team should update localized setup guides (8 language versions) using English docs as reference
- New file-based samples should follow this pattern automatically
- CONTRIBUTING.MD already documents the .NET 10 file-based pattern; no updates needed
