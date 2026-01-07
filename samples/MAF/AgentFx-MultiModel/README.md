# MCP-05-AgentFx-MultiModel

This console application demonstrates the Microsoft Agent Framework with **3 different agents** working together in a sequential workflow:

1. **Researcher Agent** (GitHub Models) - Uses GitHub Models API with `gpt-4o-mini` to research topics
2. **Writer Agent** (Azure Foundry) - Uses Azure OpenAI/Azure Foundry to write content based on research
3. **Reviewer Agent** (Ollama) - Uses local Ollama with `llama3.2` model to review and provide feedback

## Prerequisites

1. **GitHub Token** for GitHub Models API
2. **Azure Foundry/OpenAI** endpoint and API key
3. **Ollama** installed and running locally with the `llama3.2` model

### Install Ollama

```bash
# Install Ollama from https://ollama.com
# Pull the llama3.2 model
ollama pull llama3.2

# Start Ollama server (if not already running)
ollama run
```

## Configuration

Set up the following user secrets or environment variables:

```bash
# For Agent 1 (Researcher using GitHub Models)
dotnet user-secrets set "GITHUB_TOKEN" "your-github-token"

# For Agent 2 (Writer using Azure Foundry)
dotnet user-secrets set "endpoint" "https://<your-endpoint>.services.ai.azure.com/"
dotnet user-secrets set "apikey" "your-azure-api-key"
dotnet user-secrets set "deploymentName" "gpt-4o-mini"  # Optional, defaults to gpt-4o-mini
```

## How to Run

```bash
cd 1-HFMCP/MCP-05-AgentFx-MultiModel
dotnet run
```

## Workflow

The application creates a sequential workflow:

```
Researcher → Writer → Reviewer
```

1. The **Researcher** agent receives the topic and gathers key facts and information
2. The **Writer** agent takes the research and creates an engaging article
3. The **Reviewer** agent analyzes the article and provides constructive feedback

## Output

The application displays the final output from all three agents working together, showing how multiple AI models can collaborate in a structured workflow.

## Key Features

- **Multi-model orchestration**: Combines different AI providers (GitHub Models, Azure Foundry, Ollama)
- **Sequential workflow**: Clear, easy-to-read agent workflow using `AgentWorkflowBuilder`
- **Flexible configuration**: Uses user secrets for secure credential management
- **Based on scenario 1**: Similar configuration patterns to other MCP scenarios
