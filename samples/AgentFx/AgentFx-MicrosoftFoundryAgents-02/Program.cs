using Azure.AI.Agents.Persistent;
using Azure.AI.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var azureFoundryProjectEndpoint = config["azureFoundryProjectEndpoint"];
var deploymentName = config["deploymentName"];
var agentName = config["agentName"];

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
        Console.WriteLine($"Agent [{agentName}]: {response.Text}");

        Console.WriteLine($"\n--- Usage ---");
        Console.WriteLine($"Total Token Count: {response.Usage.TotalTokenCount}");
        Console.WriteLine($"Input Token Count: {response.Usage.InputTokenCount}");
        Console.WriteLine($"Output Token Count: {response.Usage.OutputTokenCount}");


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