using Azure.AI.OpenAI;
using MAF_HostedAgent_02_MultiAgent.Agents;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.ClientModel;

// Configure the chat client from environment variables
IChatClient chatClient = CreateChatClient();

// Create the three collaborating agents
AIAgent researcher = ResearcherAgent.Create(chatClient);
AIAgent writer = WriterAgent.Create(chatClient);
AIAgent reviewer = ReviewerAgent.Create(chatClient);

// Build a sequential workflow: Researcher → Writer → Reviewer
Workflow workflow = AgentWorkflowBuilder.BuildSequential(researcher, writer, reviewer);
AIAgent workflowAgent = workflow.AsAIAgent();

// Interactive loop
Console.WriteLine("Multi-Agent Research Assistant");
Console.WriteLine("Workflow: Researcher → Writer → Reviewer");
Console.WriteLine("Type a topic to research, or 'exit' to quit.\n");

while (true)
{
    Console.Write("Topic: ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    Console.WriteLine("\n--- Running workflow ---");
    Console.WriteLine("[1/3] Researcher is gathering information...");
    Console.WriteLine("[2/3] Writer is drafting the article...");
    Console.WriteLine("[3/3] Reviewer is polishing the output...\n");

    AgentResponse response = await workflowAgent.RunAsync(
        $"Research and write an article about: {input}");

    Console.WriteLine("=== Final Output ===\n");
    Console.WriteLine(response.Text);
    Console.WriteLine($"\n{new string('=', 60)}\n");
}

Console.WriteLine("Goodbye!");

// --- Chat client factory ---

static IChatClient CreateChatClient()
{
    // Try Azure OpenAI first
    var azureEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
    var azureModel = Environment.GetEnvironmentVariable("AZURE_OPENAI_MODEL") ?? "gpt-4o-mini";
    var azureApiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_APIKEY");

    if (!string.IsNullOrEmpty(azureEndpoint) && !string.IsNullOrEmpty(azureApiKey))
    {
        Console.WriteLine($"Using Azure OpenAI: {azureEndpoint} / {azureModel}");
        return new AzureOpenAIClient(new Uri(azureEndpoint), new ApiKeyCredential(azureApiKey))
            .GetChatClient(azureModel)
            .AsIChatClient();
    }

    // Fall back to Ollama
    var ollamaEndpoint = Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434/";
    var ollamaModel = Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini";

    Console.WriteLine($"Using Ollama: {ollamaEndpoint} / {ollamaModel}");
    return new OllamaApiClient(new Uri(ollamaEndpoint), ollamaModel);
}
