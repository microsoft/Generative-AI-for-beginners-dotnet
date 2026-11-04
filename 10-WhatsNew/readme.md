# What's New Archive

This document contains historical updates to the Generative AI for Beginners .NET course.

## 🎯 Decision-powered Hosted Agent (November 2026)

A new .NET sample demonstrates a custom Microsoft Agent Framework `AIAgent`
that calls **Microsoft-Decision-1** through Foundry's native decision endpoint
and exposes a typed result through the Responses protocol. No chat model is
used to make the routing decision.

### Key Updates

- **New Sample:** [DecisionAgent-01-FoundryHosted](../samples/CoreSamples/DecisionAgent-01-FoundryHosted/) routes synthetic support tickets to one of four teams or manual review.
- **Provider-independent result:** confidence, per-option probabilities, and optional usage are preserved in a stable C# contract.
- **Hosted and local paths:** the same agent implementation runs locally and as a .NET Foundry Hosted Agent.
- **Remote C# caller:** checks the final Responses status before deserializing the typed decision.
- **Offline tests:** cover validation, native request/authentication, usage semantics, provider failures, cancellation, and Responses handling without Azure credentials.
- **Community dependency:** the current Foundry decision client is `ElBruno.AI.Decisions.Foundry`, not an official Microsoft.Extensions.AI API.

### Resources

- 📚 [Sample README](../samples/CoreSamples/DecisionAgent-01-FoundryHosted/README.md)
- 📖 [Foundry Hosted Agents](https://learn.microsoft.com/azure/foundry/agents/concepts/hosted-agents?WT.mc_id=academic-105485-koreyst)
- 📖 [Foundry endpoint authorization](https://learn.microsoft.com/azure/foundry/agents/how-to/configure-agent-endpoint-authorization?WT.mc_id=academic-105485-koreyst)
- 📚 [Source attribution and notices](../samples/CoreSamples/DecisionAgent-01-FoundryHosted/THIRD-PARTY-NOTICES.md)

Live deployment permissions and portal screenshots must be verified with a
real authorized Foundry project before publication. Offline tests are not
evidence of live inference or hosted identity access.

---

## 🤖 GitHub Copilot SDK with BYOK (October 2026)

A new sample demonstrates how to call the **GitHub Copilot SDK** from .NET using your own model deployment, hosted in **Microsoft Foundry** or **Azure OpenAI**.

### Key Updates

- **New Sample:** [GitHubCopilotSDK-01-BYOK](../samples/CoreSamples/GitHubCopilotSDK-01-BYOK/) — a minimal BYOK (Bring Your Own Key) console app
- **Provider Routing:** `ProviderConfig` sends inference to any OpenAI-compatible endpoint
- **Two Authentication Modes:**
  - `AzureCliCredential` by default, reusing your `az login` session
  - An optional API key that switches to the `api-key` header
- **Focused Scope:** Tools, skills, configuration discovery, and Git operations are disabled so the sample isolates a single model request
- **Shared Configuration:** Reuses the repository's `AzureOpenAI:*` user secrets, so the existing setup scripts configure it with no extra steps

### Getting Started

From the repository root, configure the shared secrets and run the sample:

```powershell
./setup-secrets.ps1 -Endpoint "https://<resource>.services.ai.azure.com" -Deployment "<your-deployment-name>"
az login
dotnet run --project ./samples/CoreSamples/GitHubCopilotSDK-01-BYOK/GitHubCopilotSDK-01-BYOK.csproj
```

### Resources

- 📚 [Sample README](../samples/CoreSamples/GitHubCopilotSDK-01-BYOK/README.md)
- 📖 [GitHub Copilot SDK documentation](https://docs.github.com/copilot)
- 🏗️ [Microsoft Foundry](https://learn.microsoft.com/azure/ai-foundry/)

---

## 🚀 Microsoft Agent Framework v1.0 GA (April 2026)

All 28 MAF samples upgraded from preview to **stable v1.0** packages. This is a major milestone for the framework and our course!

### Key Updates

- **28 Samples Updated:** All Microsoft Agent Framework samples now use stable v1.0 packages
- **Breaking Change:** `Microsoft.Agents.AI.AzureAI` has been renamed to `Microsoft.Agents.AI.Foundry` for better alignment with the Azure Foundry platform
- **2 New Hosted Agent Samples:**
  - [MAF-HostedAgent-01-TimeZone](../samples/MAF/MAF-HostedAgent-01-TimeZone/) — Basic hosted agent demonstrating timezone tool usage
  - [MAF-HostedAgent-02-MultiAgent](../samples/MAF/MAF-HostedAgent-02-MultiAgent/) — Multi-agent Research Assistant workflow showing agent collaboration
- **Production-Ready Features:**
  - Multi-agent workflows for complex scenarios
  - Streaming support for real-time responses
  - Persistence capabilities for state management
  - Model Context Protocol (MCP) integration

### Migration Guide

If you're upgrading existing samples from RC to GA:

1. Update NuGet packages to v1.0.0 stable
2. Replace `using Microsoft.Agents.AI.AzureAI` with `using Microsoft.Agents.AI.Foundry`
3. Update any project references if using project-based dependencies
4. Test your multi-agent workflows and streaming implementations

### Resources

- 📖 [Official GA Release](https://github.com/microsoft/agent-framework/releases/tag/dotnet-1.0.0)
- 🏗️ [Foundry Agent Service GA Announcement](https://devblogs.microsoft.com/foundry/foundry-agent-service-ga/)
- 📚 [MAF Samples Directory](../samples/MAF/)

---

## 🚀 Microsoft Agent Framework Reaches Release Candidate (Previous Update)

Microsoft Agent Framework reached **Release Candidate** (`1.0.0-rc1`), bringing a framework for building agents and multi-agent systems in .NET. The course includes 25+ Agent Framework samples covering:

- Console applications
- Web-based chat interfaces
- Multi-agent workflows
- Foundry integration

### Resources

- 📚 [Explore the Agent Framework Samples](../samples/MAF/)
- 📖 [RC Announcement](https://devblogs.microsoft.com/foundry/microsoft-agent-framework-reaches-release-candidate/)

---

## 🤖 Optional provider-specific update: Claude with Agent Framework (Previous Update)

Claude-via-Foundry samples remain in the repository as optional reference material. They are still useful if you specifically want Anthropic coverage, but they are **not** part of the current recommended learning path.

### Resources

- 📚 [Claude MAF Samples](../samples/MAF/CLAUDE-SAMPLES-README.md)
- 💬 [BasicChat with Claude via Microsoft Foundry](../samples/CoreSamples/BasicChat-11FoundryClaude/)
