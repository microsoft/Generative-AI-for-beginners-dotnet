#:package OllamaSharp@5.5.0
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.14
#:property JsonSerializerIsReflectionEnabledByDefault=true

using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Text;

using var httpClient = new HttpClient
{
    BaseAddress = new Uri(Environment.GetEnvironmentVariable("OLLAMA_ENDPOINT") ?? "http://localhost:11434"),
    Timeout = TimeSpan.FromMinutes(2)
};
using IChatClient client =
    new OllamaApiClient(httpClient, Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "phi4-mini");

// here we're building the prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list including the original text and the sentiment, and then provide a generate sentiment of all reviews. ");
prompt.AppendLine("I bought this product and it's amazing. I love it!");
prompt.AppendLine("This product is terrible. I hate it.");
prompt.AppendLine("I'm not sure about this product. It's okay.");
prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

// send the prompt to the model and wait for the text completion
using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(2));
var response = await client.GetResponseAsync(prompt.ToString(), cancellationToken: timeout.Token);
if (string.IsNullOrWhiteSpace(response.Text))
{
    throw new InvalidOperationException("The model returned no text.");
}

Console.WriteLine(response.Text);
