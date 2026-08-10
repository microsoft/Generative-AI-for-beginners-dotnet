using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AzureOpenAI:Endpoint"] ?? throw new InvalidOperationException(
    "Missing 'AzureOpenAI:Endpoint'. Run: dotnet user-secrets set \"AzureOpenAI:Endpoint\" \"https://<your-resource>.openai.azure.com/\"");
var deploymentName = config["AzureOpenAI:Deployment"] ?? "gpt-5-mini";

IChatClient chatClient =
    new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
        .GetChatClient(deploymentName)
        .AsIChatClient();

AIAgent writer = chatClient.AsAIAgent(
    name: "Writer",
    instructions: "Write stories that are engaging and creative.");

// Create a specialized editor agent
AIAgent editor = chatClient.AsAIAgent(
    name: "Editor",
    instructions: "Make the story more engaging, fix grammar, and enhance the plot.");

// Create a workflow that connects writer to editor
Workflow workflow =
    AgentWorkflowBuilder
        .BuildSequential(writer, editor);

AIAgent workflowAgent = workflow.AsAIAgent();

// Stream the workflow response so the story appears token-by-token in the
// console — a livelier demo experience than waiting for the full response.
// Print a header whenever the speaking agent changes, so the Writer -> Editor
// handoff is visible instead of arriving as one anonymous wall of text.
string? currentAuthor = null;
await foreach (var update in workflowAgent.RunStreamingAsync(
    "Write a short story about a haunted house. Keep it under 200 words."))
{
    if (update.AuthorName is { Length: > 0 } author && author != currentAuthor)
    {
        currentAuthor = author;
        Console.WriteLine();
        Console.WriteLine($"=== {author} ===");
    }

    Console.Write(update.Text);
}
Console.WriteLine();