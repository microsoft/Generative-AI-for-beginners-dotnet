using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Text;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["endpoint"];
var endpointClaude = config["endpointClaude"];
var apiKey = config["apikey"];
var deploymentName = config["deploymentName"];

// 1. create custom http client that will handle Claude endpoint in Azure
var customHttpMessageHandler = new ClaudeToOpenAIMessageHandler
{
    AzureClaudeDeploymentUrl = endpointClaude,
    ApiKey = apiKey, // Pass the API key to the handler
    Model = deploymentName // Pass the model name to the handler
};
HttpClient customHttpClient = new(customHttpMessageHandler);

// 2. Wrap HttpClient in the NEW pipeline transport
var transport = new HttpClientPipelineTransport(customHttpClient);

// 3. Client options (generational)
var clientOptions = new AzureOpenAIClientOptions
{
    Transport = transport
};

// 4. Credential type for generational client
var apiKeyCredential = new ApiKeyCredential(apiKey);

// 5. Create the client with the custom transport and credential
IChatClient client = new AzureOpenAIClient(
    endpoint: new Uri(endpoint),
    credential: apiKeyCredential,
    options: clientOptions)
    .GetChatClient(deploymentName)
    .AsIChatClient()
    .AsBuilder()
    .Build();

var history = new List<ChatMessage>
{
    new(ChatRole.System, "You are a useful chatbot.")
};

while (true)
{
    Console.Write("Q: ");
    var userQ = Console.ReadLine();
    if (string.IsNullOrEmpty(userQ))
    {
        break;
    }
    history.Add(new ChatMessage(ChatRole.User, userQ));

    var sb = new StringBuilder();
    var result = client.GetStreamingResponseAsync(history);
    Console.Write($"AI [{deploymentName}]: ");
    await foreach (var item in result)
    {
        // validate if the item is null or has no contents
        if (item == null || item.Contents.Count == 0)
        {
            continue; // skip to the next item if it's null or empty
        }
        sb.Append(item);
        Console.Write(item.Contents[0].ToString());
    }
    Console.WriteLine();

    history.Add(new ChatMessage(ChatRole.Assistant, sb.ToString()));
}