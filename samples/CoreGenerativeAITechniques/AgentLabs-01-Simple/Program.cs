using Azure;
using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

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

// create Agent
var agentResponse = await persistentClient.Administration.CreateAgentAsync(
   model: "gpt-4.1",
    name: "Math Tutor",
    instructions: "You are a personal math tutor. Write and run code to answer math questions.",
    tools: [new CodeInterpreterToolDefinition()]);
var agentMathTutor = agentResponse.Value;

// Create thread for communication
PersistentAgentThread thread = await persistentClient.Threads.CreateThreadAsync();

// user question
var question = "My name is Bruno, I need to solve the equation `3x + 11 = 14`. Can you help me?";
PersistentThreadMessage message = await persistentClient.Messages.CreateMessageAsync(
    thread.Id,
    MessageRole.User,
    $"{question}");

// agent task to answer the question
var runResponse = await persistentClient.Runs.CreateRunAsync(thread, agentMathTutor);

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
await persistentClient.Administration.DeleteAgentAsync(agentMathTutor.Id);
Console.WriteLine($"Agent {agentMathTutor.Name} deleted.");