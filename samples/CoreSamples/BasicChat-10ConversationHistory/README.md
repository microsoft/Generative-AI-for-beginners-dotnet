# BasicChat-10ConversationHistory

## Understanding Chat Message Lists and GetResponseAsync

This example demonstrates the **correct way** to handle conversation history when using `GetResponseAsync()` with Microsoft.Extensions.AI.

## Common Issue: Adding Response to Conversation

When building a chatbot with conversation history, developers often encounter this error:

```csharp
List<ChatMessage> conversation = new() { ... };
var response = await client.GetResponseAsync(conversation);
conversation.Add(response.Messages);  // ❌ ERROR: response doesn't have .Messages property
```

### Why This Error Occurs

The `GetResponseAsync()` method returns a response object that has:
- ✅ `.Text` property - Contains the text response from the AI
- ✅ `.Message` property - Contains a single ChatMessage object
- ❌ NO `.Messages` property (plural)

### The Correct Pattern

Here's how to properly maintain conversation history:

```csharp
// 1. Initialize conversation with system message
List<ChatMessage> conversation = new()
{
    new ChatMessage(ChatRole.System, "You are a helpful assistant")
};

// 2. Add user message
conversation.Add(new ChatMessage(ChatRole.User, userQuestion));

// 3. Get AI response
var response = await client.GetResponseAsync(conversation);

// 4. Add assistant response to history - THREE CORRECT WAYS:

// Option A: Using response.Text (most common)
conversation.Add(new ChatMessage(ChatRole.Assistant, response.Text));

// Option B: Using response.Message (contains the full message object)
conversation.Add(response.Message);

// Option C: Using the Message property directly for more control
var assistantMessage = new ChatMessage(ChatRole.Assistant, response.Text);
conversation.Add(assistantMessage);
```

## Key Properties of Response Object

When you call `GetResponseAsync()`, the response object provides:

| Property | Type | Description |
|----------|------|-------------|
| `.Text` | `string` | The text content of the AI's response |
| `.Message` | `ChatMessage` | A ChatMessage object with Role=Assistant and the response text |
| `.Contents` | `IList<AIContent>` | Collection of content items (text, images, etc.) |
| `.FinishReason` | `ChatFinishReason?` | Reason why the response ended |

## Running This Example

1. Ensure Ollama is running:
   ```bash
   ollama serve
   ```

2. Pull the required model:
   ```bash
   ollama pull phi4:latest
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Type "conversation" to start chatting, then enter your questions.

## Alternative: Using GetStreamingResponseAsync

For streaming responses (showing text as it arrives), use a different pattern:

```csharp
var sb = new StringBuilder();
await foreach (var item in client.GetStreamingResponseAsync(conversation))
{
    sb.Append(item);
    Console.Write(item.Contents[0].ToString());
}
conversation.Add(new ChatMessage(ChatRole.Assistant, sb.ToString()));
```

See `BasicChat-05AIFoundryModels` for a complete streaming example.

## Common Mistakes to Avoid

❌ **Wrong:** `conversation.Add(response.Messages)` - `.Messages` doesn't exist  
❌ **Wrong:** `conversation.Add(response)` - Can't add response object directly  
❌ **Wrong:** `conversation.Add(response.Text)` - Can't add string directly  

✅ **Correct:** `conversation.Add(new ChatMessage(ChatRole.Assistant, response.Text))`  
✅ **Correct:** `conversation.Add(response.Message)`

## See Also

- [Microsoft.Extensions.AI Documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.ai)
- BasicChat-01MEAI - Simple single-turn conversation
- BasicChat-05AIFoundryModels - Streaming conversation with history
