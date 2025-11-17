# AgentFx AIWebChatApp with AG-UI Integration

This sample demonstrates how to build a Blazor web application that uses the **Microsoft Agent Framework** with **AG-UI** (Agent Gateway User Interface) to create a distributed AI agent architecture. The sample showcases how to separate the AI agent backend from the frontend, enabling better scalability and maintainability.

## Overview

This application consists of three main components orchestrated using .NET Aspire:

1. **Agent Backend Service** (`AgentFx-AIWebChatApp-AG-UI-Agents`) - Hosts the AI agent and exposes it via AG-UI endpoints
2. **Blazor Frontend** (`AgentFx-AIWebChatApp-AG-UI.Web`) - Interactive web UI that connects to the remote agent
3. **Aspire App Host** (`AgentFx-AIWebChatApp-AG-UI.AppHost`) - Orchestrates the distributed application

### Architecture

```text
┌─────────────────────┐
│   Blazor Web App    │
│  (Frontend Client)  │
│                     │
│  - Chat UI          │
│  - Local Tools      │
│  - Vector Search    │
└──────────┬──────────┘
           │
           │ HTTP/HTTPS
           │ AG-UI Protocol
           │
┌──────────▼──────────┐
│   Agent Backend     │
│  (Remote Service)   │
│                     │
│  - AI Agent         │
│  - Azure OpenAI     │
│  - Agent Logic      │
└─────────────────────┘
```

## Key Features

### AG-UI Integration

This sample demonstrates the **AG-UI** feature of Microsoft Agent Framework, which allows:

- **Remote Agent Execution**: The AI agent runs as a separate backend service, not embedded in the frontend
- **AGUIChatClient**: A specialized chat client that communicates with remote agents over HTTP
- **Distributed Architecture**: Frontend and backend can scale independently
- **Tool Distribution**: Frontend can provide its own tools (like search) while the backend handles AI reasoning

### Semantic Search with RAG

The application includes:

- **PDF Document Ingestion**: Automatically processes PDF files from `wwwroot/Data` directory
- **Vector Store**: Uses SQLite with vector embeddings for semantic search
- **Search Function**: AI agent can search through ingested documents to provide contextual answers
- **Citation Support**: Responses include citations with filename and page numbers

### Aspire Orchestration

Built with .NET Aspire for:

- Service discovery and communication
- Automatic Azure OpenAI provisioning (in publish mode)
- Health checks and observability
- Development and production configurations

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download) or later
- Access to **Azure OpenAI** with:
  - `gpt-4o-mini` (or `gpt-5-mini`) model deployment
  - `text-embedding-3-small` model deployment
- Visual Studio 2022 or Visual Studio Code with C# extensions

## Setup Instructions

### 1. Configure Azure OpenAI

You need to provide Azure OpenAI credentials. Choose one of the following methods:

#### Option A: User Secrets (Recommended for Local Development)

Navigate to the **AppHost** project directory and set user secrets:

```powershell
cd AgentFx-AIWebChatApp-AG-UI.AppHost

# Set Azure OpenAI connection string
dotnet user-secrets set "ConnectionStrings:openai" "Endpoint=https://YOUR-RESOURCE.openai.azure.com/;Key=YOUR-API-KEY"
```

#### Option B: Environment Variables

Set the following environment variable:

```powershell
$env:ConnectionStrings__openai = "Endpoint=https://YOUR-RESOURCE.openai.azure.com/;Key=YOUR-API-KEY"
```

#### Option C: Aspire Configuration (For Azure Deployment)

When using `builder.ExecutionContext.IsPublishMode`, the AppHost automatically provisions Azure OpenAI resources. See [Aspire Azure provisioning documentation](https://learn.microsoft.com/dotnet/aspire/azure/local-provisioning#configuration?wt.mc_id=dotnet-153583-brunocapuano) for details.

### 2. Add Your Data (Optional)

Place PDF files you want to search in:

```
AgentFx-AIWebChatApp-AG-UI.Web/wwwroot/Data/
```

The sample includes two example PDFs:

- `Example_Emergency_Survival_Kit.pdf`
- `Example_GPS_Watch.pdf`

**Important**: Ensure any content you ingest is trusted, as it may be reflected back to users or could be a source of prompt injection risk.

## Running the Application

### Using Visual Studio

1. Open `AgentFx-AIWebChatApp-AG-UI.slnx`
2. Set **AgentFx-AIWebChatApp-AG-UI.AppHost** as the startup project
3. Press `F5` to run

### Using .NET CLI

```powershell
cd AgentFx-AIWebChatApp-AG-UI.AppHost
dotnet run
```

The Aspire dashboard will open, showing:

- **aichatweb-agents** - The agent backend service
- **aichatweb-app** - The Blazor web frontend

Navigate to the **aichatweb-app** endpoint to access the chat interface.

## Project Structure

### AgentFx-AIWebChatApp-AG-UI-Agents (Backend)

The agent backend service that hosts the AI agent:

```csharp
// Register AI Agent with AG-UI endpoint
builder.AddAIAgent("ChatAgent", (sp, key) =>
{
    var chatClient = sp.GetRequiredService<IChatClient>();
    
    var aiAgent = chatClient.CreateAIAgent(
        name: key,
        instructions: "You are a useful agent that helps users with short and funny answers.",
        description: "An AI agent that helps users with short and funny answers."
    )
    .AsBuilder()
    .UseOpenTelemetry(configure: c =>
        c.EnableSensitiveData = builder.Environment.IsDevelopment())
    .Build();

    return aiAgent;
});

// Map AG-UI endpoint
var aiAgent = app.Services.GetKeyedService<AIAgent>("ChatAgent");
app.MapAGUI("/", aiAgent);
```

**Key Points:**

- Hosts the AI agent as a web service
- Exposes AG-UI endpoints via `app.MapAGUI()`
- Configures Azure OpenAI chat client with function invocation support
- Enables OpenTelemetry for observability

### AgentFx-AIWebChatApp-AG-UI.Web (Frontend)

The Blazor frontend that connects to the remote agent:

```csharp
// Create AGUIChatClient to communicate with remote agent
var agentService = sp.GetRequiredService<AgentsService>();
AGUIChatClient chatClient = new(agentService.HttpClient, "https+http://aichatweb-agents");

// Create AI Agent with frontend tools
var searchFunctions = sp.GetRequiredService<SearchFunctions>();
AITool[] frontendTools = [AIFunctionFactory.Create(searchFunctions.SearchAsync)];

var aiAgent = chatClient.CreateAIAgent(
    name: "ChatAgent",
    instructions: "You are a useful agent that helps users with short and funny answers.",
    description: "An AI agent that helps users with short and funny answers.",
    tools: frontendTools
)
.AsBuilder()
.UseOpenTelemetry(configure: c =>
    c.EnableSensitiveData = builder.Environment.IsDevelopment())
.Build();
```

**Key Points:**

- Uses `AGUIChatClient` to connect to the remote agent backend
- Provides local tools (search functionality) that the remote agent can invoke
- Implements vector search over ingested PDF documents
- Interactive Blazor Server UI with streaming responses

### Search Functionality

The frontend provides a search tool that the AI agent can use:

```csharp
[Description("Searches for information using a phrase or keyword")]
public async Task<IEnumerable<string>> SearchAsync(
    [Description("The phrase to search for.")] string searchPhrase,
    [Description("If possible, specify the filename to search that file only.")] string? filenameFilter = null)
{
    // Perform semantic search over ingested chunks
    var results = await _semanticSearch.SearchAsync(searchPhrase, filenameFilter, maxResults: 5);

    // Return formatted results as XML
    return results.Select(result =>
        $"<result filename=\"{result.DocumentId}\" page_number=\"{result.PageNumber}\">{result.Text}</result>");
}
```

**How It Works:**

1. User asks a question in the chat UI
2. The frontend sends the message to the remote agent via AGUIChatClient
3. The remote agent processes the request and may call the frontend's search tool
4. Frontend executes semantic search against vector store
5. Results are sent back to the remote agent
6. Agent formulates a response with citations
7. Response is streamed back to the frontend UI

## AG-UI Protocol

The AG-UI feature enables a distributed agent architecture where:

1. **Backend Service**: Hosts the AI agent logic and LLM integration
2. **Frontend Client**: Provides UI and local tools/data access
3. **Communication**: HTTP-based protocol for agent invocation and tool calling

### Benefits

- **Separation of Concerns**: UI logic separate from AI agent logic
- **Scalability**: Backend agent service can scale independently
- **Security**: Keep sensitive API keys and credentials in backend only
- **Tool Distribution**: Frontend can provide tools that access local data or user context

### MapAGUI vs AGUIChatClient

- **`MapAGUI()`**: Used in the backend to expose an agent via HTTP endpoints
- **`AGUIChatClient`**: Used in the frontend to connect to a remote agent exposed via MapAGUI

## Key Technologies

- **[Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/?wt.mc_id=dotnet-153583-brunocapuano)**: AI agent orchestration and management
- **[AG-UI Integration](https://learn.microsoft.com/agent-framework/integrations/ag-ui/?pivots=programming-language-csharp&wt.mc_id=dotnet-153583-brunocapuano)**: Remote agent communication protocol
- **[Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions?wt.mc_id=dotnet-153583-brunocapuano)**: AI service abstractions for .NET
- **[.NET Aspire](https://learn.microsoft.com/dotnet/aspire?wt.mc_id=dotnet-153583-brunocapuano)**: Cloud-native application orchestration
- **[Blazor Server](https://learn.microsoft.com/aspnet/core/blazor?wt.mc_id=dotnet-153583-brunocapuano)**: Interactive web UI with streaming support
- **[Semantic Kernel](https://learn.microsoft.com/semantic-kernel?wt.mc_id=dotnet-153583-brunocapuano)**: Vector store integration
- **[Azure OpenAI](https://learn.microsoft.com/azure/ai-services/openai?wt.mc_id=dotnet-153583-brunocapuano)**: LLM and embedding models

## NuGet Packages

### Backend (Agents Service)

```xml
<PackageReference Include="Microsoft.Agents.AI" Version="1.0.0-preview.251114.1" />
<PackageReference Include="Microsoft.Agents.AI.Hosting" Version="1.0.0-preview.251114.1" />
<PackageReference Include="Microsoft.Agents.AI.Hosting.AGUI.AspNetCore" Version="1.0.0-preview.251114.1" />
<PackageReference Include="Microsoft.Extensions.AI" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.AI.OpenAI" Version="10.0.0-preview.1.25560.10" />
<PackageReference Include="Aspire.Azure.AI.OpenAI" Version="13.0.0-preview.1.25560.3" />
```

### Frontend (Web App)

```xml
<PackageReference Include="Microsoft.Agents.AI" Version="1.0.0-preview.251114.1" />
<PackageReference Include="Microsoft.Agents.AI.AGUI" Version="1.0.0-preview.251114.1" />
<PackageReference Include="Microsoft.Agents.AI.Hosting" Version="1.0.0-preview.251114.1" />
<PackageReference Include="Microsoft.Extensions.AI" Version="10.0.0" />
<PackageReference Include="Microsoft.SemanticKernel.Connectors.SqliteVec" Version="1.67.1-preview" />
<PackageReference Include="PdfPig" Version="0.1.13-alpha-20251115-aef0a" />
```

## Learn More

- [Microsoft Agent Framework Documentation](https://learn.microsoft.com/agent-framework/?wt.mc_id=dotnet-153583-brunocapuano)
- [AG-UI Integration Guide](https://learn.microsoft.com/agent-framework/integrations/ag-ui/?pivots=programming-language-csharp&wt.mc_id=dotnet-153583-brunocapuano)
- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire?wt.mc_id=dotnet-153583-brunocapuano)
- [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/ai-extensions?wt.mc_id=dotnet-153583-brunocapuano)
- [Semantic Kernel](https://learn.microsoft.com/semantic-kernel?wt.mc_id=dotnet-153583-brunocapuano)

## Troubleshooting

### Common Issues

**"Unable to connect to agent service"**

- Ensure both projects are running (check Aspire dashboard)
- Verify the service URL in `AgentsService` matches the Aspire service name
- Check that the agent backend is listening on the correct port

**"No search results found"**

- Verify PDF files are in `wwwroot/Data` directory
- Check that the vector store was created (look for `vector-store.db` in output directory)
- Ensure the embedding model is correctly configured

**"OpenAI API errors"**

- Verify your Azure OpenAI connection string is correct
- Ensure both model deployments (`gpt-4o-mini` and `text-embedding-3-small`) exist
- Check your Azure OpenAI quota and rate limits

**"Build errors with Microsoft.Agents.AI packages"**

- Ensure you're using .NET 9 SDK or later
- Clear NuGet cache: `dotnet nuget locals all --clear`
- Restore packages: `dotnet restore`

## License

This sample is part of the [Generative AI for Beginners - .NET Edition](../../README.md) course.

---

For more Agent Framework samples, see:

- [AgentFx Lesson 06](../../../06-AgentFx/readme.md)
- [Other AgentFx Samples](../README.md)
