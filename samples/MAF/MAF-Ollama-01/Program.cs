using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

IChatClient chatClient =
    new OllamaApiClient(new Uri("http://localhost:11434/"), "llama3.2");

AIAgent writer = chatClient.AsAIAgent(
    name: "Writer",
    instructions: "Write short stories that are engaging and creative, and always add bad jokes to them.");

AgentResponse response = await writer.RunAsync("Write a long story about Lima Peru en Spanish");

Console.WriteLine(response.Text);