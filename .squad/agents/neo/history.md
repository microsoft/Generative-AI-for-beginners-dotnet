# Neo — History

## Project Context
- **Project:** Generative AI for Beginners .NET — hands-on .NET course for GenAI
- **Stack:** .NET 9+, C#, MEAI, Semantic Kernel, Azure OpenAI, Ollama
- **User:** Bruno Capuano

## Learnings
- As of July 2025, no standalone MEAI-native NuGet packages exist for InMemory, AzureAISearch, Qdrant, or SqliteVec vector stores. The SK connector packages remain the only implementations.
- `Microsoft.SemanticKernel.Text.TextChunker` has no MEAI-native replacement. SK Core is still required for text chunking.
- `AddSqliteCollection<>()` extension method is provided by `Microsoft.SemanticKernel.Connectors.SqliteVec` with no MEAI equivalent.
- The Middleware MAF project has a pre-existing NU1605 package downgrade error (MEAI 10.2.0 vs 9.10.1) unrelated to connector changes.

### Phase 1 — Azure Infrastructure & Scripts
- Created `infra/` with Bicep templates: `main.bicep` orchestrator + 3 modules (`cognitive-services.bicep`, `model-deployment-chat.bicep`, `model-deployment-embedding.bicep`).
- Models deployed: `gpt-5-mini` (GlobalStandard, 10K TPM) and `text-embedding-3-small` (Standard, 10K TPM).
- Created `azure.yaml` for `azd` integration (Bicep provider, infra path).
- Created `setup.ps1`: prereq checks → `azd up` → endpoint extraction → user secrets (`--id genai-beginners-dotnet`) with Endpoint, Deployment, and EmbeddingDeployment → tenant detection.
- Created `cleanup.ps1`: `azd down --purge --force` → remove `.azure/` → clear user secrets.
- Secrets ID for this project: `genai-beginners-dotnet`.
- Reference pattern: modeled after `Azure-Samples/agent-skills-dotnet-demo` setup/cleanup scripts.
- Key file paths: `infra/main.bicep`, `infra/main.parameters.json`, `infra/modules/`, `azure.yaml`, `setup.ps1`, `cleanup.ps1`.

### Phase 2 — Migrate CoreSamples to Shared Secrets & Model
- **UserSecretsId updated to `genai-beginners-dotnet`** in 26 .csproj files:
  - 24 existing UserSecretsId replaced: BasicChat-01MEAI, BasicChat-05AIFoundryModels, BasicChat-06OpenAIAPIs, BasicChat-08LLMTornado, BasicChat-10ConversationHistory, BasicChat-11FoundryClaude, MEAIFunctions, MEAIFunctionsAzureOpenAI, Vision-01MEAI-AzureOpenAI, Vision-01MEAI-GitHubModels, Vision-03MEAI-AOAI, Vision-04MEAI-AOAI-Spectre, ImageGeneration-01, Audio-01-SpeechMic, Audio-02-RealTimeAudio, AgentLabs-01-Simple, AgentLabs-02-Functions, AgentLabs-03-OpenAPIs, AIToolkit-02-MEAI-Chat, MCP-01-HuggingFace, RAGSimple-03MEAIVectorsAISearch, VideoGeneration-AzureSora-01, VideoGeneration-AzureSora2-01, VideoGeneration-AzureSoraSDK-02
  - 2 new UserSecretsId added: RAGSimple-02MEAIVectorsMemory, RAGSimple-04MEAIVectorsQdrant
- **Model names migrated to `config["AzureOpenAI:Deployment"]`** in 13 Program.cs files:
  - BasicChat-01MEAI, BasicChat-05AIFoundryModels, BasicChat-06OpenAIAPIs, MEAIFunctions, MEAIFunctionsAzureOpenAI, Vision-01MEAI-AzureOpenAI, Vision-01MEAI-GitHubModels, Vision-03MEAI-AOAI, Vision-04MEAI-AOAI-Spectre, AgentLabs-01-Simple, AgentLabs-02-Functions, AgentLabs-03-OpenAPIs, MCP-01-HuggingFace
- **Skipped (Ollama/local-only):** BasicChat-03Ollama, BasicChat-07Ollama-gpt-oss, BasicChat-09LLMTornadoOllama, Vision-02MEAI-Ollama, AIFoundryLocal-01-MEAI-Chat, DockerModels-02-MEAI-Chat, MEAIFunctionsOllama, MCP-02-HuggingFace-Ollama
- **Skipped model update for:** ImageGeneration-01 (dall-e-3), Video generation projects (sora/sora-2), Audio-01-SpeechMic (Speech service), Audio-02-RealTimeAudio (gpt-realtime), BasicChat-08LLMTornado (library enum), BasicChat-10ConversationHistory (Ollama), BasicChat-11FoundryClaude (Claude), AIToolkit-02-MEAI-Chat (local model), RAG projects (embedding models only), MEAIVectorsShared (shared library)
- **Transitive dependencies must be tracked explicitly**: BasicChat-11FoundryClaude was missing explicit `Azure.AI.OpenAI` reference because SK provided it transitively. When removing SK, explicit references are not added automatically—manual audit required.
- **Verify the pattern, not just the fix**: BasicChat-05AIFoundryModels already had the correct fix applied (Azure.AI.OpenAI 2.8.0-beta.1). Use this as a template when similar issues arise in other projects.
- **Document patterns for future developers**: Created commit message guidance: when removing a major package, check (1) what transitive deps it provides, (2) does code directly reference types from those deps, (3) add explicit refs before removal.
- **Build verification is defensive**: The fix is simple (one line XML), but it protects course users from a build failure on a prominent sample. Defensive QA at this level saves support overhead.

### Phase 3 — Migrate MAF samples to Shared Secrets & Model
- **UserSecretsId updated to `genai-beginners-dotnet`** in 29 .csproj files across samples/MAF/:
  - Console apps: MAF01, MAF02, MAF-AIFoundry-01, MAF-AIFoundry-02, MAF-AIFoundryAgents-01, MAF-MicrosoftFoundryAgents-01, MAF-MicrosoftFoundryAgents-02, MAF-MultiModel, MAF-MultiAgents, MAF-ImageGen-01, MAF-ImageGen-02, MAF-Persisting-01-Simple, MAF-Persisting-02-Menu, MAF-BackgroundResponses-01-Simple, MAF-BackgroundResponses-02-Tools, MAF-BackgroundResponses-03-Complex, MAF-FoundryClaude-01, MAF-FoundryClaude-Persisting-01
  - Web apps: MAF-AIWebChatApp-FoundryClaude, MAF-AIWebChatApp-Persisting (.Web + .AppHost), MAF-AIWebChatApp-Simple (ChatApp20.Web + ChatApp20.AppHost), MAF-AIWebChatApp-MutliAgent (ChatApp20.Web + ChatApp20.AppHost), MAF-AIWebChatApp-Middleware (ChatApp20.Web + ChatApp20.AppHost), MAF-AIWebChatApp-AG-UI (.Web + .AppHost)
- **Model config key migrated from `config["deploymentName"]` to `config["AzureOpenAI:Deployment"]`** in 13 .cs files:
  - MAF01/Program.cs, MAF02/Program.cs, MAF-AIFoundry-01/Program.cs, MAF-AIFoundry-02/Program.cs, MAF-AIFoundryAgents-01/Program.cs, MAF-MicrosoftFoundryAgents-01/Program.cs, MAF-MicrosoftFoundryAgents-02/Program.cs
  - MAF-BackgroundResponses-01-Simple/ChatClientProvider.cs, MAF-BackgroundResponses-01-Simple/ResponseClientProvider.cs (shared by Persisting-01, Persisting-02, BackgroundResponses-02, BackgroundResponses-03 via linked files)
  - MAF-ImageGen-01/ChatClientProvider.cs, MAF-ImageGen-02/ChatClientProvider.cs, MAF-MultiModel/ChatClientProvider.cs, MAF-MultiAgents/AppConfigurationService.cs
- **Hardcoded `gpt-4o-mini` updated to `gpt-5-mini`** in:
  - MAF01, MAF02, MAF-AIFoundry-01, MAF-AIFoundry-02 (console apps with fallback defaults)
  - 4 Aspire AppHost files: MAF-AIWebChatApp-MutliAgent, MAF-AIWebChatApp-Persisting, MAF-AIWebChatApp-Middleware, MAF-AIWebChatApp-AG-UI (AddDeployment name/modelName + modelVersion)
  - MAF-AIWebChatApp-AG-UI-Agents/Program.cs (AddChatClient + log message)
  - MAF-AIWebChatApp-AG-UI.Web/Program.cs (log message)
  - Comment updates in MAF-MultiAgents/Program.cs and MAF-MultiModel/Program.cs
- **Skipped (per task instructions):**
  - MAF-Ollama-01 (Ollama only, no secrets)
  - MAF-MultiAgents-Factory-01 (Ollama only, no Azure references)
  - Claude models in MAF-FoundryClaude-01, MAF-FoundryClaude-Persisting-01, MAF-AIWebChatApp-FoundryClaude (keep `claude-haiku-4-5`)
  - Ollama model names (`llama3.2`) in MAF-MultiModel, MAF-MultiAgents
  - Image generation models (dall-e-3) in MAF-ImageGen-01/02
  - ServiceDefaults .csproj files (no UserSecretsId, no Azure references)

### Phase 4 — .NET 10 File-Based App Migration + setup-secrets.ps1
- **Migrated 13 CoreSamples to .NET 10 file-based apps** (`dotnet run app.cs`, no .csproj):
  - BasicChat-01MEAI, BasicChat-03Ollama, BasicChat-05AIFoundryModels, BasicChat-06OpenAIAPIs, BasicChat-07Ollama-gpt-oss, BasicChat-08LLMTornado, BasicChat-09LLMTornadoOllama, BasicChat-10ConversationHistory, BasicChat-11FoundryClaude, ImageGeneration-01, MEAIFunctions, MEAIFunctionsAzureOpenAI, AgentLabs-01-Simple
- **Migration pattern**: Read .csproj for PackageReferences → create `app.cs` with `#:package Name@Version` directives + `#:property UserSecretsId=genai-beginners-dotnet` (where applicable) + original Program.cs code → delete .csproj, Program.cs, bin/, obj/
- **Ollama-only projects** (BasicChat-03Ollama, BasicChat-07Ollama-gpt-oss, BasicChat-09LLMTornadoOllama) do NOT include `#:property UserSecretsId`
- **Multi-file merge**: BasicChat-11FoundryClaude had `ClaudeToOpenAIMessageHandler.cs` merged into `app.cs` after the top-level statements (file-based apps compile single file only)
- **Created `setup-secrets.ps1`** at repo root: manual user secrets configuration script (alternative to `azd`). Takes `-Endpoint` (required), `-Deployment` (default: gpt-5-mini), `-EmbeddingDeployment` (default: text-embedding-3-small). Sets secrets under ID `genai-beginners-dotnet`.
