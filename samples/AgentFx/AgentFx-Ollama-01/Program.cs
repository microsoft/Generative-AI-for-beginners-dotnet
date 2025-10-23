using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient chatClient =
    new OllamaApiClient(new Uri("http://localhost:11434/"), "llama3.2");

AIAgent writer = new ChatClientAgent(
    chatClient,
    new ChatClientAgentOptions
    {
        Name = "Writer",
        Instructions = "Write stories that are engaging and creative."
    });

AgentRunResponse response = await writer.RunAsync("Write a short story about a haunted house.");

Console.WriteLine(response.Text);