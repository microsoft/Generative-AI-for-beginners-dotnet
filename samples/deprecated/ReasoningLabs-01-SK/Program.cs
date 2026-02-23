#pragma warning disable SKEXP0010 // Reasoning effort is still in preview for OpenAI SDK.

using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// ==============================
// Using Azure OpenAI models
// ==============================
//var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT");
//var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
//var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_APIKEY");
//var modelId = Environment.GetEnvironmentVariable("AZURE_OPENAI_MODEL");
//if (string.IsNullOrEmpty(endpoint))
//{
//    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
//    deploymentName = config["AZURE_OPENAI_DEPLOYMENT"];
//    endpoint = config["AZURE_OPENAI_ENDPOINT"];
//    apiKey = config["AZURE_OPENAI_APIKEY"];
//    modelId = config["AZURE_OPENAI_MODEL"];
//}

//// Initialize the OpenAI chat completion service with the o3-mini model.
//var chatService = new AzureOpenAIChatCompletionService(
//    deploymentName: deploymentName, 
//    endpoint: endpoint,
//    apiKey: apiKey,
//    modelId: modelId
//);

// ==============================
// Using OpenAI API directly
// ==============================
// Initialize the OpenAI chat completion service with the o3-mini model.
var chatService = new OpenAIChatCompletionService(
    modelId: "o3-mini",  // OpenAI API endpoint
    apiKey: ""  // Your OpenAI API key
);

// Create a new chat history and add a user message to prompt the model.
ChatHistory chatHistory = [];
chatHistory.AddUserMessage("Why is the sky blue in one sentence?");

// Configure reasoning effort for the chat completion request.
var settings = new OpenAIPromptExecutionSettings { ReasoningEffort = "high" };

// Send the chat completion request to o3-mini
var reply = await chatService.GetChatMessageContentAsync(chatHistory, settings);
Console.WriteLine("o3-mini reply: " + reply);

