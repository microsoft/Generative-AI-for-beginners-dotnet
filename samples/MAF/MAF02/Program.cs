using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["endpoint"];
var apiKey = new ApiKeyCredential(config["apikey"]);
var deploymentName = config["deploymentName"] ?? "gpt-4o-mini";

IChatClient chatClient =
    new AzureOpenAIClient(new Uri(endpoint), apiKey)
        .GetChatClient(deploymentName)
        .AsIChatClient();

AIAgent writer = chatClient.CreateAIAgent(
    name: "Writer",
    instructions: "Write stories that are engaging and creative.");

// Create a specialized editor agent
AIAgent editor = chatClient.CreateAIAgent(
    name: "Editor",
    instructions: "Make the story more engaging, fix grammar, and enhance the plot.");

// Create a workflow that connects writer to editor
Workflow workflow =
    AgentWorkflowBuilder
        .BuildSequential(writer, editor);

AIAgent workflowAgent = workflow.AsAgent();

AgentRunResponse workflowResponse =
    await workflowAgent.RunAsync("Write a short story about a haunted house.");

Console.WriteLine(workflowResponse.Text);