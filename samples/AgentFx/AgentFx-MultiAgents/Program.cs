using AgentFx_MultiAgents;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenTelemetry;
using OpenTelemetry.Trace;


// ================================================================
// CONFIGURATION REQUIREMENTS
// ================================================================
// This demo requires the following user secrets or environment variables:
//
// REQUIRED - Azure AI Foundry Persistent Agent (Agent 1 - Researcher):
//      "AZURE_FOUNDRY_PROJECT_ENDPOINT": "https://<your-project>.services.ai.azure.com/"
//      "deploymentName": "your-model-deployment-name" (default: "gpt-5-mini")
//
// REQUIRED - Agent 2 (Writer) - One of the following options:
//
//   Option A - GitHub Models:
//      "GITHUB_TOKEN": "your-github-personal-access-token"
//      "deploymentName": "model-name" (e.g., "gpt-4o-mini")
//
//   Option B - Azure OpenAI with API Key:
//      "endpoint": "https://<your-resource>.cognitiveservices.azure.com"
//      "apikey": "your-azure-openai-api-key"
//      "deploymentName": "your-deployment-name" (e.g., "gpt-4o-mini")
//
//   Option C - Azure OpenAI with Default Credentials (fallback):
//      "endpoint": "https://<your-resource>.cognitiveservices.azure.com"
//      "deploymentName": "your-deployment-name"
//      Note: Requires Azure CLI login (az login) or managed identity
//
// REQUIRED - Ollama (Agent 3 - Reviewer):
//      Ollama must be running locally on http://localhost:11434/
//      with the 'llama3.2' model downloaded and available.
//
// Configuration Priority for Agent 2:
//      1. GitHub Models (if GITHUB_TOKEN is set)
//      2. Azure OpenAI with API Key (if apikey is set)
//      3. Azure OpenAI with Default Credentials (fallback)
//
// To set user secrets, run:
//      dotnet user-secrets set "AZURE_FOUNDRY_PROJECT_ENDPOINT" "https://your-project.services.ai.azure.com/"
//      dotnet user-secrets set "deploymentName" "gpt-4o-mini"
//      dotnet user-secrets set "endpoint" "https://your-resource.cognitiveservices.azure.com"
//      dotnet user-secrets set "apikey" "your-api-key"
//
// To set up Ollama:
//      1. Install Ollama from https://ollama.ai/
//      2. Run: ollama pull llama3.2
//      3. Ensure Ollama service is running
// ================================================================

Console.WriteLine("=== Microsoft Agent Framework - Multi-Model Orchestration Demo ===");
Console.WriteLine("This demo showcases 3 agents working together:");
Console.WriteLine("  1. Researcher (Azure AI Foundry Agent) - Researches topics");
Console.WriteLine("  2. Writer (Azure OpenAI or GitHub Models) - Writes content based on research");
Console.WriteLine("  3. Reviewer (Ollama - llama3.2) - Reviews and provides feedback");
Console.WriteLine();

// ===== OpenTelemetry Trace Provider ====
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("agent-telemetry-source")
    .AddConsoleExporter()
    .Build();

// ===== Agent 1: Researcher using Azure OpenAI or GitHub Models =====
Console.WriteLine("Setting up Agent 1: Researcher (Azure AI Foundry Agent)...");

AIAgent researcher = AIFoundryAgentsProvider.CreateAIAgent(
    name: "Researcher",
    instructions: "You are a research expert. Your job is to gather key facts and interesting points about the given topic. Be concise and focus on the most important information.")
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "agent-telemetry-source")
    .Build();

// ===== Agent 2: Writer using Azure OpenAI or GitHub Models =====
Console.WriteLine("Setting up Agent 2: Writer (Azure OpenAI or GitHub Models)...");

IChatClient azureChatClient = ChatClientProvider.GetChatClient();

AIAgent writer = new ChatClientAgent(
    azureChatClient,
    new ChatClientAgentOptions
    {
        Name = "Writer",
        Instructions = "You are a creative writer. Take the research provided and write an engaging, well-structured article. Make it informative yet entertaining."
    })
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "agent-telemetry-source")
    .Build();


// ===== Agent 3: Reviewer using Ollama =====
Console.WriteLine("Setting up Agent 3: Reviewer (Ollama)...");
IChatClient ollamaChatClient = ChatClientProvider.GetChatClientOllama();

AIAgent reviewer = new ChatClientAgent(
    ollamaChatClient,
    new ChatClientAgentOptions
    {
        Name = "Reviewer",
        Instructions = "You are an editor and reviewer. Analyze the article provided, give constructive feedback, and suggest improvements for clarity, grammar, and engagement."
    })
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "agent-telemetry-source")
    .Build();

// ===== Create Sequential Workflow =====
Console.WriteLine("Creating workflow: Researcher -> Writer -> Reviewer");
Console.WriteLine();

Workflow workflow =
    AgentWorkflowBuilder
        .BuildSequential(researcher, writer, reviewer);

AIAgent workflowAgent = await workflow.AsAgentAsync();

// ===== Execute the Workflow =====
var topic = "artificial intelligence in healthcare";
Console.WriteLine($"Starting workflow with topic: '{topic}'");
Console.WriteLine(new string('=', 80));
Console.WriteLine();

AgentRunResponse workflowResponse =
    await workflowAgent.RunAsync($"Research and write an article about: {topic}");

Console.WriteLine("=== Final Output ===");
Console.WriteLine(workflowResponse.Text);
Console.WriteLine();
Console.WriteLine(new string('=', 80));
Console.WriteLine("Workflow completed successfully!");

Console.WriteLine("=== Clean Up ===");
Console.WriteLine("Do you want to delete the Researcher agent in Azure AI Foundry? (yes/no)");
string deleteResponse = Console.ReadLine()?.Trim().ToLower() ?? "no";
if (deleteResponse == "yes" || deleteResponse == "y")
{
    Console.WriteLine("Deleting Researcher agent in Azure AI Foundry...");
    AIFoundryAgentsProvider.DeleteAIAgentInAIFoundry(researcher);
    Console.WriteLine("Researcher agent deleted successfully.");
}
else
{
    Console.WriteLine("Researcher agent not deleted.");
}
Console.WriteLine();
