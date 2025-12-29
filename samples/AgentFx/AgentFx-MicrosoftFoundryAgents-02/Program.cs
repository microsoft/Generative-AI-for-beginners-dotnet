using Azure.AI.Agents.Persistent;
using Azure.AI.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;

#pragma warning disable CA2252 // Opt-in for preview features used by AIProjectClient in samples
using System.Diagnostics;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var azureFoundryProjectEndpoint = config["azureFoundryProjectEndpoint"] ?? throw new InvalidOperationException("Missing 'azureFoundryProjectEndpoint' configuration");
var deploymentName = config["deploymentName"] ?? throw new InvalidOperationException("Missing 'deploymentName' configuration");
var agentName = config["agentName"] ?? throw new InvalidOperationException("Missing 'agentName' configuration");

AIProjectClient projectClient = new(
    endpoint: new Uri(azureFoundryProjectEndpoint),
    tokenProvider: new AzureCliCredential());

Console.WriteLine("=== AI Agent Session ===");
Console.WriteLine($"Project Endpoint: {azureFoundryProjectEndpoint}");
Console.WriteLine($"Model Deployment: {deploymentName}");
Console.WriteLine($"Agent Name: {agentName}");
Console.WriteLine();

// create agent (example with tools)
//AIAgent aiAgent = projectClient.CreateAIAgent(
//    model: deploymentName,
//    name: "Agent",
//    instructions: "You are a useful agent that replies in short and direct sentences.");

// get existing agent
Console.WriteLine($"Retrieving agent '{agentName}'...");
AIAgent aiAgent = await projectClient.GetAIAgentAsync(agentName);
Console.WriteLine($"✓ Agent retrieved: {aiAgent.Id}");
Console.WriteLine();

while (true)
{
    Console.Write("User: ");
    var userInput = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userInput))
    {
        break;
    }

    var stopwatch = Stopwatch.StartNew();

    try
    {
        Console.WriteLine($"\n[Running agent...]");

        // Run the agent with the user input
        var response = await aiAgent.RunAsync(userInput);
        Console.WriteLine(response.Text);

        stopwatch.Stop();

        // Display run details
        Console.WriteLine($"\n--- Run Details ---");
        Console.WriteLine($"Duration: {stopwatch.ElapsedMilliseconds}ms");

        // Display the response
        Console.WriteLine($"\n--- Response ---");
        var responseText = response.Text ?? string.Empty;
        Console.WriteLine($"Agent [{agentName}]: {responseText}");

        Console.WriteLine($"\n--- Usage ---");
        var totalTokens = response.Usage?.TotalTokenCount ?? 0;
        var inputTokens = response.Usage?.InputTokenCount ?? 0;
        var outputTokens = response.Usage?.OutputTokenCount ?? 0;
        Console.WriteLine($"Total Token Count: {totalTokens}");
        Console.WriteLine($"Input Token Count: {inputTokens}");
        Console.WriteLine($"Output Token Count: {outputTokens}");


        Console.WriteLine($"\n{'='.ToString().PadRight(50, '=')}");

    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"\n❌ Error: {ex.Message}");
        Console.WriteLine($"Type: {ex.GetType().Name}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"Inner: {ex.InnerException.Message}");
        }
    }

    Console.WriteLine();
}

Console.WriteLine("\nSession ended. Goodbye!");