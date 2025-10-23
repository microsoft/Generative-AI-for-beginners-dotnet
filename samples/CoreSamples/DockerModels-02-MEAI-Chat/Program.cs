using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text;

var model = "ai/deepseek-r1-distill-llama";
var base_url = "http://localhost:12434/engines/llama.cpp/v1";
var api_key = "unused";

OpenAIClientOptions options = new OpenAIClientOptions();
options.Endpoint = new Uri(base_url);
ApiKeyCredential credential = new ApiKeyCredential(api_key);

ChatClient client = new OpenAIClient(credential, options).GetChatClient(model);

// here we're building the prompt
StringBuilder prompt = new StringBuilder();
prompt.AppendLine("You will analyze the sentiment of the following product reviews. Each line is its own review. Output the sentiment of each review in a bulleted list and then provide a generate sentiment of all reviews. ");
prompt.AppendLine("I bought this product and it's amazing. I love it!");
prompt.AppendLine("This product is terrible. I hate it.");
prompt.AppendLine("I'm not sure about this product. It's okay.");
prompt.AppendLine("I found this product based on the other reviews. It worked for a bit, and then it didn't.");

// send the prompt to the model and wait for the text completion
var response = await client.CompleteChatAsync(prompt.ToString());

// display the response
Console.WriteLine(response.Value.Content[0].Text);