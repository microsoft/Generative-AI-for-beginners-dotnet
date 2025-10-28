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
        Instructions = "Write short stories that are engaging and creative, and always add bad jokes to them."
    });

AgentRunResponse response = await writer.RunAsync("Write a short story about .NET Conf.");

Console.WriteLine(response.Text);