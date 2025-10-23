using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

// to run this sample, you need to set up the following user secrets to the project:
//{
//  "aifoundryproject_tenantid": "< your tenant id >",
//  "aifoundryproject_endpoint": "https://<...>.services.ai.azure.com/api/projects/<...>",
//}

// Create Agent Client
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var tenantid = config["aifoundryproject_tenantid"];
var aifoundryproject_endpoint = config["aifoundryproject_endpoint"];

var options = new DefaultAzureCredentialOptions
{
    ExcludeEnvironmentCredential = true,
    ExcludeWorkloadIdentityCredential = true,
    TenantId = tenantid
};

PersistentAgentsClient persistentClient = new(aifoundryproject_endpoint, new DefaultAzureCredential(options));

OpenApiAnonymousAuthDetails oaiAuth = new();
OpenApiToolDefinition parksinformationOpenApiTool = new(
    name: "get_park_information",
    description: "Retrieve parks information for a location",
    spec: BinaryData.FromBytes(File.ReadAllBytes(@"./specs/parksinformationopenapi.json")),
    openApiAuthentication: oaiAuth
);

// create Agent
var agentResponse = await persistentClient.Administration.CreateAgentAsync(
   model: "gpt-4.1",
    name: "SDK Test Agent - Vacation",
    instructions: @"You are a travel assistant. Use the provided functions to help answer questions. 
Customize your responses to the user's preferences as much as possible. Write and run code to answer user questions.",
    tools: new List<ToolDefinition> { parksinformationOpenApiTool }
    );
var agentTravelAssistant = agentResponse.Value;

// Create thread for communication
PersistentAgentThread thread = await persistentClient.Threads.CreateThreadAsync();

// user question
var question = "My name is Bruno, I want to know the weather in Seattle, and also information from the parks in the city";
PersistentThreadMessage message = await persistentClient.Messages.CreateMessageAsync(
    thread.Id,
    MessageRole.User,
    $"{question}");


// agent task to answer the question
var runResponse = await persistentClient.Runs.CreateRunAsync(thread, agentTravelAssistant);

// run the agent thread
do
{
    await Task.Delay(TimeSpan.FromMilliseconds(500));
    runResponse = await persistentClient.Runs.GetRunAsync(thread.Id, runResponse.Value.Id);
}
while (runResponse.Value.Status == RunStatus.Queued
    || runResponse.Value.Status == RunStatus.InProgress);

AsyncPageable<PersistentThreadMessage> messages = persistentClient.Messages.GetMessagesAsync(
threadId: thread.Id, order: ListSortOrder.Ascending);

// show the response
Console.WriteLine("==========================");
await foreach (PersistentThreadMessage threadMessage in messages)
{
    Console.Write($"{threadMessage.CreatedAt:yyyy-MM-dd HH:mm:ss} - {threadMessage.Role,10}: ");
    foreach (MessageContent contentItem in threadMessage.ContentItems)
    {
        if (contentItem is MessageTextContent textItem)
        {
            Console.Write(textItem.Text);
        }
        else if (contentItem is MessageImageFileContent imageFileItem)
        {
            Console.Write($"<image from ID: {imageFileItem.FileId}");
        }
        Console.WriteLine();
    }
}
Console.WriteLine("==========================");

// delete the agent after use
await persistentClient.Administration.DeleteAgentAsync(agentTravelAssistant.Id);
Console.WriteLine($"Agent {agentTravelAssistant.Name} deleted.");