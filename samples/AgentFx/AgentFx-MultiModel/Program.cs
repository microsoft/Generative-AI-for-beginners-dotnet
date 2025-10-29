using AgentFx_MultiModel;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenTelemetry;
using OpenTelemetry.Trace;


// To run the sample, you need to set the following environment variables or user secrets:
// Using GitHub models (for Agent 1):
//      "GITHUB_TOKEN": "your GitHub Token"
// Using Azure Foundry/OpenAI (for Agent 2):
//      "endpoint": "https://<endpoint>.services.ai.azure.com/"
//      "apikey": "your key"
//      "deploymentName": "a deployment name, ie: gpt-4o-mini"
// Ollama should be running locally on http://localhost:11434/ with llama3.2 model (for Agent 3)

Console.WriteLine("=== Microsoft Agent Framework - Multi-Model Orchestration Demo ===");
Console.WriteLine("This demo showcases 3 agents working together:");
Console.WriteLine("  1. Researcher (Azure AI Foundry or GitHub Models) - Researches topics");
Console.WriteLine("  2. Writer (Azure AI Foundry or GitHub Models) - Writes content based on research");
Console.WriteLine("  3. Reviewer (Ollama - llama 3.2) - Reviews and provides feedback");
Console.WriteLine();

// ===== OpenTelemetry Trace Provider ====
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("agent-telemetry-source")
    .AddConsoleExporter()
    .Build();

// ===== Agent 1: Researcher using GitHub Models =====
Console.WriteLine("Setting up Agent 1: Researcher (Azure AI Foundry or GitHub Models)...");

IChatClient githubChatClient = ChatClientProvider.GetChatClient();

AIAgent researcher = new ChatClientAgent(
    githubChatClient,
    new ChatClientAgentOptions
    {
        Name = "Researcher",
        Instructions = "You are a research expert. Your job is to gather key facts and interesting points about the given topic. Be concise and focus on the most important information."
    })
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "agent-telemetry-source")
    .Build(); 

// ===== Agent 2: Writer using Azure Foundry/OpenAI =====
Console.WriteLine("Setting up Agent 2: Writer (Azure AI Foundry or GitHub Models)...");

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

AIAgent workflowAgent = workflow.AsAgent();

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
