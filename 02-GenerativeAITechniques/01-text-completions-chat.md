# Text Completions and Chat Conversations

In this lesson, you will learn the fundamentals of interacting with AI models in .NET. We'll start with the simplest interaction - text completions - and build up to full conversational applications.

---

[![Text Completions and Chat](./images/LIM_GAN_03_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson2-chat)

_Click the image to watch the video_

---

## From "Hello World" to Real Conversations

In the previous lesson, you learned that calling an AI model is just like calling an API. Let's prove it.

By the end of this section, you'll build a chat application that:

- Remembers what you said earlier in the conversation
- Follows instructions you give it (like "be brief" or "respond in Spanish")
- Maintains full conversational context

And here's the best part: the core pattern fits in about 15 lines of code.

```csharp
// A complete chat application with memory
List<ChatMessage> conversation = new()
{
    new ChatMessage(ChatRole.System, "You are a helpful assistant.")
};

while (true)
{
    Console.Write("You: ");
    conversation.Add(new ChatMessage(ChatRole.User, Console.ReadLine()!));
    
    var response = await chatClient.GetResponseAsync(conversation);
    conversation.Add(new ChatMessage(ChatRole.Assistant, response.Text));
    
    Console.WriteLine($"AI: {response.Text}");
}
```

That's a working conversational AI. Let's break down how it works.

---

## Part 1: Text Completions - The Simplest AI Interaction

Before we build conversations, let's start with the simplest possible interaction: a single prompt and response.

### What Is a Text Completion?

A **text completion** is a one-shot interaction. You send a prompt, you get a response. Done.

Think of it like asking someone a question in passing. You're not starting a conversation, just getting a quick answer.

**Best for:**

- Summarization
- Sentiment analysis
- Classification
- One-time transformations

### Your First Completion

Here's how to get a text completion with Microsoft.Extensions.AI:

```csharp
IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(githubToken))
        .AsIChatClient("gpt-4o-mini");

var response = await client.GetResponseAsync(
    "Summarize the benefits of cloud computing in one sentence.");

Console.WriteLine(response.Text);
```

**What's happening here:**

| Line | What It Does |
|------|-------------|
| `ChatCompletionsClient` | Creates a connection to GitHub Models |
| `.AsIChatClient("gpt-4o-mini")` | Wraps it in the standard `IChatClient` interface |
| `GetResponseAsync(prompt)` | Sends your prompt, waits for the response |
| `response.Text` | The AI's generated text |

> **Try it yourself:** [BasicChat-01MEAI sample](../samples/CoreSamples/BasicChat-01MEAI/)

### How to Run the Sample Code

To run the sample code, you'll need to:

1. Make sure you have set up your development environment as described in the [Setup guide](../01-IntroductionToGenerativeAI/readme.md)
2. Ensure you have configured your GitHub Token or other credentials
3. Open a terminal in your IDE
4. Navigate to the sample code directory:

   ```bash
   cd samples/CoreSamples/BasicChat-01MEAI
   ```

5. Run the application:

   ```bash
   dotnet run
   ```

### A Practical Example: Sentiment Analysis

Let's do something useful. Analyze the sentiment of customer reviews:

```csharp
StringBuilder prompt = new();
prompt.AppendLine("Analyze the sentiment of each review. Output: Review number, Sentiment (Positive/Negative/Neutral), Key reason.");
prompt.AppendLine();
prompt.AppendLine("Review 1: I bought this product and it's amazing. I love it!");
prompt.AppendLine("Review 2: This product is terrible. I hate it.");
prompt.AppendLine("Review 3: I'm not sure about this product. It's okay.");

var response = await client.GetResponseAsync(prompt.ToString());
Console.WriteLine(response.Text);
```

**Expected output:**

```
Review 1: Positive - Strong enthusiasm ("amazing", "love it")
Review 2: Negative - Strong dissatisfaction ("terrible", "hate it")
Review 3: Neutral - Uncertainty and lukewarm response ("not sure", "okay")
```

Notice how we didn't write any sentiment analysis code. We described what we wanted, and the model did the rest.

---

## Part 2: From Completion to Conversation

Text completions are useful, but most AI applications need **conversations**. The model needs to remember what was said before.

### The Problem: AI Has No Memory

By default, each call to an AI model is independent. The model doesn't remember previous calls.

```csharp
// First call
await client.GetResponseAsync("My name is Bruno.");
// Response: "Nice to meet you, Bruno!"

// Second call
await client.GetResponseAsync("What's my name?");
// Response: "I don't know your name. You haven't told me."
```

The second call has no idea about the first one.

### The Solution: You Manage the Memory

To create a conversation, you maintain a **list of messages** and send the entire history with each request:

```csharp
List<ChatMessage> conversation = new();

// Add user's first message
conversation.Add(new ChatMessage(ChatRole.User, "My name is Bruno."));

// Get response and add it to history
var response1 = await client.GetResponseAsync(conversation);
conversation.Add(new ChatMessage(ChatRole.Assistant, response1.Text));

// Now ask the follow-up question
conversation.Add(new ChatMessage(ChatRole.User, "What's my name?"));

var response2 = await client.GetResponseAsync(conversation);
// Response: "Your name is Bruno."
```

Now the model "remembers" because you sent the entire conversation each time.

### Visualizing Conversation Flow

Here's what's happening:

```
Request 1: [User: "My name is Bruno."]
Response 1: "Nice to meet you, Bruno!"

Request 2: [User: "My name is Bruno.", Assistant: "Nice to meet you, Bruno!", User: "What's my name?"]
Response 2: "Your name is Bruno."
```

The history grows with each exchange. The model sees the full context every time.

### Using AddMessages Helper

The Microsoft.Extensions.AI library provides a convenient helper method to add response messages to your history:

```csharp
List<ChatMessage> history = [];
while (true)
{
    Console.Write("Q: ");
    history.Add(new(ChatRole.User, Console.ReadLine()));

    ChatResponse response = await client.GetResponseAsync(history);
    Console.WriteLine(response);

    history.AddMessages(response);  // Adds all messages from the response
}
```

The `AddMessages` extension method extracts messages from the `ChatResponse` and adds them to your history list.

---

## Part 3: The Three Chat Roles

Every message in a conversation has a **role**. Understanding these roles is key to building effective AI applications.

| Role | Who Creates It | Purpose |
|------|---------------|---------|
| **System** | You (the developer) | Sets the AI's behavior, personality, and constraints |
| **User** | The end user | Questions, commands, or input |
| **Assistant** | The AI model | Responses generated by the model |

### The System Role: Your Secret Weapon

The **system message** is the most powerful tool for controlling AI behavior. It's the first message in the conversation and sets the rules.

```csharp
List<ChatMessage> conversation = new()
{
    new ChatMessage(ChatRole.System, 
        "You are a senior .NET developer. Answer questions about C# and .NET. " +
        "Keep responses concise. Use code examples when helpful. " +
        "If asked about other topics, politely redirect to .NET topics.")
};
```

**What this system message does:**

- **Sets expertise:** "senior .NET developer"
- **Defines scope:** "C# and .NET"
- **Sets style:** "concise", "code examples"
- **Sets boundaries:** redirect non-.NET questions

### System Message Patterns

Here are common patterns for system messages:

**The Expert:**

```csharp
"You are an expert in [domain]. Your role is to [specific task]."
```

**The Persona:**

```csharp
"You are a friendly customer service agent named Alex. Be helpful but brief."
```

**The Constrained Assistant:**

```csharp
"Answer only questions about [topic]. For other topics, say 'I can only help with [topic].'"
```

**The Format Controller:**

```csharp
"Respond only in JSON format with these fields: { 'answer': string, 'confidence': number }"
```

### Why This Matters

The system message runs **once** at the start, but influences **every** response. It's your main control mechanism.

> **Try it yourself:** [BasicChat-10ConversationHistory sample](../samples/CoreSamples/BasicChat-10ConversationHistory/)

---

## Part 4: Building a Complete Chat Application

Let's put it all together. Here's a complete, working chat application:

```csharp
using Microsoft.Extensions.AI;

// Create the AI client (using GitHub Models)
IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(Environment.GetEnvironmentVariable("GITHUB_TOKEN")!))
        .AsIChatClient("gpt-4o-mini");

// Initialize conversation with a system message
List<ChatMessage> conversation = new()
{
    new ChatMessage(ChatRole.System, 
        "You are a helpful assistant. Be concise but friendly.")
};

Console.WriteLine("Chat started. Type 'quit' to exit.\n");

while (true)
{
    Console.Write("You: ");
    var userInput = Console.ReadLine();
    
    if (string.IsNullOrEmpty(userInput) || userInput.ToLower() == "quit")
        break;
    
    // Add user message to history
    conversation.Add(new ChatMessage(ChatRole.User, userInput));
    
    // Get AI response
    var response = await client.GetResponseAsync(conversation);
    
    // Add AI response to history
    conversation.AddMessages(response);
    
    Console.WriteLine($"AI: {response.Text}\n");
}
```

**Key points:**

1. The `conversation` list persists across the loop
2. Every user message is added before the API call
3. Every AI response is added after using `AddMessages`
4. The system message shapes all responses

### Switching Providers

The same code works with any `IChatClient` provider. Just change the client instantiation:

```csharp
// Option 1: GitHub Models
IChatClient client = new ChatCompletionsClient(...)
    .AsIChatClient("gpt-4o-mini");

// Option 2: Ollama (local)
IChatClient client = new OllamaChatClient(
    new Uri("http://localhost:11434"), "phi4-mini");

// Option 3: Azure OpenAI
IChatClient client = new AzureOpenAIChatClient(
    endpoint, credential, "gpt-4o");
```

Your conversation logic stays exactly the same.

---

## Let's Review: What You Learned

| Concept | Key Takeaway |
|---------|-------------|
| **Text Completions** | Single prompt, single response. Good for one-off tasks. |
| **Chat Conversations** | Maintain a message list to give the AI memory. |
| **System Role** | Controls AI behavior, personality, and constraints. Set once, affects all responses. |
| **User/Assistant Roles** | User provides input, Assistant provides responses. Both stored in history. |

### Quick Self-Check

Can you answer these questions?

1. Why does the AI "forget" between calls if you don't maintain a conversation list?
2. What's the purpose of the System role, and when is it set?
3. How does `AddMessages` help manage conversation history?

---

## Sample Code Reference

| Sample | Description |
|--------|-------------|
| [BasicChat-01MEAI](../samples/CoreSamples/BasicChat-01MEAI/) | Text completion with GitHub Models |
| [BasicChat-02SK](../samples/CoreSamples/BasicChat-02SK/) | Chat with Semantic Kernel |
| [BasicChat-03Ollama](../samples/CoreSamples/BasicChat-03Ollama/) | Chat with local Ollama |
| [BasicChat-10ConversationHistory](../samples/CoreSamples/BasicChat-10ConversationHistory/) | Managing conversation history |

---

## Additional Resources

- [IChatClient Interface Guide](https://learn.microsoft.com/dotnet/ai/ichatclient)
- [Build an AI Chat App with .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/build-chat-app)

---

## Up Next

Now that you understand the basics of text completions and conversations, let's explore more advanced capabilities:

[Continue to Part 2: Streaming and Structured Output](./02-streaming-structured-output.md)
