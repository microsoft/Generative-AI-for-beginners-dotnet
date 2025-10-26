using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Text;

var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if(string.IsNullOrEmpty(githubToken))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    githubToken = config["GITHUB_TOKEN"];
}

IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(githubToken))
        .AsIChatClient("Phi-4-mini-instruct");

// here we're building the prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a generate sentiment of all reviews. ");
prompt.AppendLine("I bought this product and it's amazing. I love it!");
prompt.AppendLine("This product is terrible. I hate it.");
prompt.AppendLine("I'm not sure about this product. It's okay.");
prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

// send the prompt to the model and wait for the text completion
var response = await client.GetResponseAsync(prompt.ToString());

// display the response
Console.WriteLine(response.Text);
