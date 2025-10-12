using LlmTornado;
using LlmTornado.Chat;
using LlmTornado.Chat.Models;
using System.Text;

TornadoApi ollamaApi = new(new Uri("http://localhost:11434"));
Conversation chat = ollamaApi.Chat.CreateConversation(
    new ChatModel("deepseek-r1:8b"));

// Prompt to test a reasoning model capability
var prompt = new StringBuilder();
prompt.AppendLine("You are a helpful assistant.");
prompt.AppendLine("Answer the following question with a short explanation:");
prompt.AppendLine("Why is the sky blue?");

chat.AppendUserInput(prompt.ToString());
string? response = await chat.GetResponse();

Console.WriteLine(response);