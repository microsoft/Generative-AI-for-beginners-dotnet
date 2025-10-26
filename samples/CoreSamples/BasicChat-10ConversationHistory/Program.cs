using Microsoft.Extensions.AI;

// Create chat client (using Ollama as an example, but this works with any IChatClient)
IChatClient client = new OllamaChatClient(new Uri("http://localhost:11434"), "phi4-mini");

// Create a conversation history list
// This is the CORRECT way to initialize a conversation with a system message
List<ChatMessage> conversation = new()
{
    new ChatMessage(ChatRole.System, "You are a good assistance with short and smart answers")
};

bool loopCheck = true;

while (loopCheck)
{
    Console.WriteLine("conversation/press any key to Exit app");
    var askCommand = Console.ReadLine();

    if (askCommand == "conversation")
    {
        // Get user input
        string question = Console.ReadLine() ?? "";
        
        // Add user message to conversation history
        conversation.Add(new ChatMessage(ChatRole.User, question));

        // Get response from the AI model
        var response = await client.GetResponseAsync(conversation);

        // IMPORTANT: The response object has a .Text property, NOT .Messages
        // To add the assistant's response to the conversation history:
        // CORRECT way:
        conversation.Add(new ChatMessage(ChatRole.Assistant, response.Text));

        // INCORRECT way (this will cause a compile error):
        // conversation.Add(response.Messages);  // ❌ response doesn't have .Messages property
        
        Console.WriteLine($"AI: {response.Text}");
    }
    else
    {
        loopCheck = false;
    }
}
