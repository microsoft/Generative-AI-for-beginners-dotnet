# MAF-MultiAgents - Multi-Model Orchestration Demo

This sample demonstrates how to orchestrate multiple AI agents using different AI providers in a sequential workflow with the Microsoft Agent Framework.

## Objective

This demo showcases a real-world scenario where three specialized agents collaborate to research, write, and review an article:

1. **Researcher Agent** (Microsoft Foundry Persistent Agent) - Researches topics and gathers key information
2. **Writer Agent** (Azure OpenAI) - Creates engaging content based on research
3. **Reviewer Agent** (Ollama local model) - Reviews the article and provides constructive feedback

The workflow demonstrates:

- **Multi-provider orchestration**: Using different AI services (Microsoft Foundry, Azure OpenAI, and Ollama) in a single workflow
- **Sequential agent workflows**: Agents execute in order, with each building on the previous agent's output
- **Persistent agents**: Creating and managing agents in Microsoft Foundry
- **OpenTelemetry tracing**: Monitoring agent execution with distributed tracing
- **Flexible configuration**: Supporting multiple authentication methods and providers

## Architecture

```
User Input
    ↓
Researcher (Microsoft Foundry Agent)
    ↓ (research findings)
Writer (Azure OpenAI)
    ↓ (article draft)
Reviewer (Ollama - llama3.2)
    ↓
Final Output (reviewed article with feedback)
```

## Prerequisites

Before running this sample, ensure you have:

1. **.NET 9 SDK** or later installed
2. **Azure CLI** installed and authenticated (`az login`)
3. **Ollama** installed and running locally with the `llama3.2` model
4. **Microsoft Foundry Project** with access to deploy models
5. One of the following for Agent 2 (Writer):
      - Azure OpenAI endpoint and API key
   - Azure OpenAI endpoint with managed identity/Azure CLI credentials

## Setup Instructions

### Step 1: Install and Configure Ollama

Ollama is required for the Reviewer agent (Agent 3).

```bash
# Install Ollama from https://ollama.com

# Pull the llama3.2 model
ollama pull llama3.2

# Verify Ollama is running (should respond on http://localhost:11434/)
ollama run llama3.2
```

### Step 2: Configure User Secrets

This application uses .NET user secrets to store sensitive configuration. Navigate to the project directory and set the required secrets:

```bash
cd 06-MAF/src/MAF-MultiAgents
```

#### Required: Microsoft Foundry Configuration (Agent 1 - Researcher)

```bash
# Set your Microsoft Foundry project endpoint
dotnet user-secrets set "AZURE_FOUNDRY_PROJECT_ENDPOINT" "https://<your-project>.services.ai.azure.com/"

# Set your model deployment name (default: gpt-5-mini)
dotnet user-secrets set "deploymentName" "gpt-4o-mini"
```

#### Required: Writer Agent Configuration (Agent 2)

Choose **ONE** of the following options:



```bash

dotnet user-secrets set "deploymentName" "gpt-4o-mini"
```

To create a GitHub token:

1. Go to <https://github.com/settings/tokens>
2. Click "Generate new token (classic)"
3. Select scopes as needed
4. Copy the token and use it above

**Option B: Azure OpenAI with API Key**

```bash
dotnet user-secrets set "endpoint" "https://<your-resource>.cognitiveservices.azure.com"
dotnet user-secrets set "apikey" "your-azure-openai-api-key"
dotnet user-secrets set "deploymentName" "gpt-4o-mini"
```

**Option C: Azure OpenAI with Managed Identity** (Fallback)

```bash
dotnet user-secrets set "endpoint" "https://<your-resource>.cognitiveservices.azure.com"
dotnet user-secrets set "deploymentName" "gpt-4o-mini"
```

Note: This option requires Azure CLI login (`az login`) or a configured managed identity.

### Step 3: Verify Azure CLI Authentication

The Researcher agent uses Azure CLI credentials to authenticate with Microsoft Foundry:

```bash
# Login to Azure CLI
az login

# Verify your authentication
az account show
```

## Configuration Priority

The application automatically selects the chat client provider in the following order:

2. **Azure OpenAI with API Key** - If `apikey` is set
3. **Azure OpenAI with Default Credentials** - Fallback using Azure CLI or managed identity

## Running the Application

Once all secrets are configured, run the application:

```bash
dotnet build
dotnet run
```

### Expected Output

The console will display:

1. Agent setup messages for each of the three agents
2. Workflow creation confirmation
3. OpenTelemetry tracing output showing agent execution
4. The final article with research, writing, and review feedback
5. A prompt asking if you want to delete the persistent agent from Microsoft Foundry

### Sample Execution Flow

```
=== Microsoft Agent Framework - Multi-Model Orchestration Demo ===
This demo showcases 3 agents working together:
  1. Researcher (Microsoft Foundry Agent) - Researches topics
  2. Writer (Azure OpenAI) - Writes content based on research
  3. Reviewer (Ollama - llama3.2) - Reviews and provides feedback

Setting up Agent 1: Researcher (Microsoft Foundry Agent)...
Setting up Agent 2: Writer (Azure OpenAI)...
Setting up Agent 3: Reviewer (Ollama)...
Creating workflow: Researcher -> Writer -> Reviewer

Starting workflow with topic: 'artificial intelligence in healthcare'
================================================================================

[OpenTelemetry traces shown here...]

=== Final Output ===
[Reviewed article with feedback...]

================================================================================
Workflow completed successfully!

=== Clean Up ===
Do you want to delete the Researcher agent in Microsoft Foundry? (yes/no)
```

## Project Structure

```
MAF-MultiAgents/
├── Program.cs                      # Main workflow orchestration
├── AIFoundryAgentsProvider.cs      # Microsoft Foundry agent factory
├── ChatClientProvider.cs           # Multi-provider chat client factory
├── AppConfigurationService.cs      # Configuration management
├── MAF-MultiAgents.csproj      # Project dependencies
└── README.md                       # This file
```

## Key Components

### AIFoundryAgentsProvider

Creates and manages persistent agents in Microsoft Foundry:

- `CreateAIAgent()` - Creates a new persistent agent
- `DeleteAIAgentInAIFoundry()` - Removes agents from Microsoft Foundry

### ChatClientProvider

Factory class for creating chat clients with different providers:

- `GetChatClient()` - Returns a chat client based on available configuration (Azure OpenAI)
- `GetChatClientOllama()` - Returns an Ollama chat client for local inference

### AppConfigurationService

Centralized configuration service that reads from:

- User secrets
- Environment variables

## Customization

### Change the Topic

Edit the `topic` variable in `Program.cs`:

```csharp
var topic = "artificial intelligence in healthcare";
```

### Modify Agent Instructions

Update the `Instructions` property for any agent:

```csharp
AIAgent researcher = new ChatClientAgent(
    azureChatClient,
    new ChatClientAgentOptions
    {
        Name = "Researcher",
        Instructions = "Your custom instructions here..."
    });
```

### Use Different Models

Change the model for any provider:

- **Azure OpenAI**: Update `deploymentName` secret with your deployment name
- **Ollama**: Modify the model parameter in `GetChatClientOllama()` call

### Add More Agents

Extend the workflow by adding additional agents:

```csharp
AIAgent seoOptimizer = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "SEO Optimizer",
        Instructions = "Optimize the article for search engines."
    });

Workflow workflow = AgentWorkflowBuilder
    .BuildSequential(researcher, writer, reviewer, seoOptimizer);
```

## Troubleshooting

### Common Issues

**Issue**: "Azure CLI credentials not found"

- **Solution**: Run `az login` to authenticate with Azure CLI

**Issue**: "Ollama connection refused"

- **Solution**: Ensure Ollama is running (`ollama run llama3.2`) and accessible at <http://localhost:11434/>

**Issue**: "Model deployment not found"

- **Solution**: Verify your `deploymentName` secret matches an actual deployment in your Azure OpenAI or AI Foundry resource

**Issue**: "GitHub token authentication failed"

- **Solution**: Verify your GitHub token is valid and has the required permissions

**Issue**: "Microsoft Foundry endpoint not found"

- **Solution**: Check that your `AZURE_FOUNDRY_PROJECT_ENDPOINT` is correct and accessible

## Learn More

- [Microsoft Agent Framework Documentation](https://learn.microsoft.com/agent-framework/overview/agent-framework-overview)
- [Microsoft.Extensions.AI Documentation](https://learn.microsoft.com/dotnet/ai/ai-extensions)
- [Microsoft Foundry](https://ai.azure.com/)
- [Ollama Documentation](https://ollama.ai/docs)
- [OpenTelemetry for .NET](https://opentelemetry.io/docs/instrumentation/net/)

## Next Steps

After exploring this sample:

1. Try modifying the agent instructions to see how it affects the output
2. Experiment with different AI models for each agent
3. Add additional agents to create more complex workflows
4. Explore the OpenTelemetry traces to understand agent execution patterns
5. Integrate with other MCP servers for additional capabilities

---

**Note**: This sample uses preview versions of Microsoft Agent Framework packages. Check for updates regularly as the framework evolves.
