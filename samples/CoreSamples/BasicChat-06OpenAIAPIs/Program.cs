using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using System.Text;

var openai_apikey = Environment.GetEnvironmentVariable("APIKEY");
if (string.IsNullOrEmpty(openai_apikey))
{
    var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
    openai_apikey = config["APIKEY"];
}

IChatClient client =
    new OpenAI.Chat.ChatClient("gpt-4.1-mini", openai_apikey)
    .AsIChatClient();

// here we're building the prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a general sentiment of all reviews. ");
prompt.AppendLine("I bought this product and it's amazing. I love it!");
prompt.AppendLine("This product is terrible. I hate it.");
prompt.AppendLine("I'm not sure about this product. It's okay.");
prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

// send the prompt to the model and wait for the text completion
var response = await client.GetResponseAsync(prompt.ToString());

// display the response
Console.WriteLine(response.Text);
