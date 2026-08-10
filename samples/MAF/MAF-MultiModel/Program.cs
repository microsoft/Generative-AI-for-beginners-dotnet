using MAF_MultiModel;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenTelemetry;
using OpenTelemetry.Trace;


// To run the sample, you need to set the following environment variables or user secrets:
// Azure Foundry/OpenAI (for Agent 1 and Agent 2):
//      "endpoint": "https://<endpoint>.services.ai.azure.com/"
//      "apikey": "your key"
//      "AzureOpenAI:Deployment": "a deployment name, ie: gpt-5-mini"
// Ollama should be running locally on http://localhost:11434/ with llama3.2 model (for Agent 3)
//
// If Ollama is not running, the demo does NOT fail silently: it checks the Ollama endpoint before
// creating Agent 3, explains how to fix it, runs the remaining 2 agents (Researcher -> Writer) and
// exits with a non-zero exit code so the partial run is never reported as a full success.
//
// Exit codes: 0 = all 3 agents ran, 1 = the workflow failed, 2 = partial run (Reviewer skipped).

Console.WriteLine("=== Microsoft Agent Framework - Multi-Model Orchestration Demo ===");
Console.WriteLine("This demo showcases 3 agents working together:");
Console.WriteLine("  1. Researcher (Azure OpenAI) - Researches topics");
Console.WriteLine("  2. Writer (Azure OpenAI) - Writes content based on research");
Console.WriteLine("  3. Reviewer (Ollama - llama 3.2) - Reviews and provides feedback");
Console.WriteLine();

// ===== OpenTelemetry Trace Provider ====
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource("agent-telemetry-source")
    .AddConsoleExporter()
    .Build();

// ===== Agent 1: Researcher using Azure OpenAI =====
Console.WriteLine("Setting up Agent 1: Researcher (Azure OpenAI)...");

IChatClient githubChatClient = ChatClientProvider.GetChatClient();

AIAgent researcher = githubChatClient.AsAIAgent(
    name: "Researcher",
    instructions: "You are a research expert. Your job is to gather key facts and interesting points about the given topic. Be concise and focus on the most important information.")
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "agent-telemetry-source")
    .Build();

// ===== Agent 2: Writer using Azure Foundry/OpenAI =====
Console.WriteLine("Setting up Agent 2: Writer (Azure OpenAI)...");

IChatClient azureChatClient = ChatClientProvider.GetChatClient();

AIAgent writer = azureChatClient.AsAIAgent(
    name: "Writer",
    instructions: "You are a creative writer. Take the research provided and write an engaging, well-structured article. Make it informative yet entertaining.")
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "agent-telemetry-source")
    .Build();


// ===== Agent 3: Reviewer using Ollama =====
Console.WriteLine("Setting up Agent 3: Reviewer (Ollama)...");
Console.WriteLine($"Checking if Ollama is available at {ChatClientProvider.OllamaEndpoint} ...");

bool ollamaAvailable = await IsOllamaAvailableAsync(ChatClientProvider.OllamaEndpoint);
AIAgent? reviewer = null;

if (ollamaAvailable)
{
    Console.WriteLine("Ollama is available.");

    IChatClient ollamaChatClient = ChatClientProvider.GetChatClientOllama();

    reviewer = ollamaChatClient.AsAIAgent(
        name: "Reviewer",
        instructions: "You are an editor and reviewer. Analyze the article provided, give constructive feedback, and suggest improvements for clarity, grammar, and engagement.")
        .AsBuilder()
        .UseOpenTelemetry(sourceName: "agent-telemetry-source")
        .Build();
}
else
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 80));
    Console.WriteLine("WARNING: Ollama is NOT reachable.");
    Console.WriteLine(new string('=', 80));
    Console.WriteLine($"Agent 3 (Reviewer) needs a local Ollama server at {ChatClientProvider.OllamaEndpoint}");
    Console.WriteLine("but nothing responded there.");
    Console.WriteLine();
    Console.WriteLine("How to fix it:");
    Console.WriteLine("  1. Install Ollama from https://ollama.com/download");
    Console.WriteLine("  2. Make sure the Ollama server is running (it starts automatically after install)");
    Console.WriteLine($"  3. Download the model with: ollama pull {ChatClientProvider.OllamaModel}");
    Console.WriteLine("  4. Run this sample again");
    Console.WriteLine();
    Console.WriteLine("Agents 1 and 2 (Researcher and Writer) use Azure OpenAI and are NOT affected,");
    Console.WriteLine("so the demo continues with a 2-agent workflow and the Reviewer step SKIPPED.");
    Console.WriteLine(new string('=', 80));
    Console.WriteLine();
}


// ===== Create Sequential Workflow =====
Console.WriteLine(ollamaAvailable
    ? "Creating workflow: Researcher -> Writer -> Reviewer"
    : "Creating workflow: Researcher -> Writer (Reviewer SKIPPED)");
Console.WriteLine();

Workflow workflow = reviewer is null
    ? AgentWorkflowBuilder.BuildSequential(researcher, writer)
    : AgentWorkflowBuilder.BuildSequential(researcher, writer, reviewer);

AIAgent workflowAgent = workflow.AsAIAgent();

// ===== Execute the Workflow =====
var topic = "artificial intelligence in healthcare";
Console.WriteLine($"Starting workflow with topic: '{topic}'");
Console.WriteLine(new string('=', 80));
Console.WriteLine();

AgentResponse workflowResponse;
try
{
    workflowResponse = await workflowAgent.RunAsync($"Research and write an article about: {topic}");
}
catch (Exception ex)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 80));
    Console.WriteLine($"Workflow FAILED while running the {(ollamaAvailable ? "Researcher -> Writer -> Reviewer" : "Researcher -> Writer")} workflow.");
    Console.WriteLine($"Error: {ex.GetType().Name}: {ex.Message}");
    Console.WriteLine("Check that your Azure OpenAI settings are configured and that Ollama is running.");
    Console.WriteLine(new string('=', 80));
    return 1;
}

Console.WriteLine("=== Final Output ===");
Console.WriteLine(workflowResponse.Text);
Console.WriteLine();
Console.WriteLine(new string('=', 80));

if (!ollamaAvailable)
{
    Console.WriteLine("Workflow completed PARTIALLY (2/3 agents). The Reviewer agent was SKIPPED because Ollama was not reachable.");
    return 2;
}

bool reviewerContributed = workflowResponse.Messages
    .Any(message => string.Equals(message.AuthorName, "Reviewer", StringComparison.OrdinalIgnoreCase));

if (!reviewerContributed)
{
    Console.WriteLine("Workflow completed PARTIALLY (2/3 agents). No output was produced by the Reviewer agent.");
    return 2;
}

Console.WriteLine("Workflow completed successfully! (3/3 agents)");
return 0;


static async Task<bool> IsOllamaAvailableAsync(string endpoint)
{
    try
    {
        using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        using var response = await httpClient.GetAsync(endpoint);
        return response.IsSuccessStatusCode;
    }
    catch (Exception)
    {
        return false;
    }
}
