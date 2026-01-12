# Streaming and Structured Output

In this lesson, you will learn how to stream AI responses in real-time and how to get strongly-typed structured output from AI models. These are essential techniques for building responsive, production-ready AI applications.

---

[![Streaming and Structured Output](./images/LIM_GAN_03_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson2-streaming)

_Click the image to watch the video_

---

## Why Streaming Matters

When you call `GetResponseAsync`, you wait for the entire response before displaying anything. For short responses, this is fine. But for longer responses, the user stares at a blank screen for several seconds.

**Streaming** lets you display tokens as they arrive, creating a more responsive experience, like what you see in ChatGPT or Copilot.

---

## Part 1: Streaming Responses

### The Basic Streaming Pattern

Instead of `GetResponseAsync`, use `GetStreamingResponseAsync`:

```csharp
await foreach (ChatResponseUpdate update in 
    client.GetStreamingResponseAsync("Explain quantum computing in simple terms"))
{
    Console.Write(update.Text);
}
```

Notice the difference:

| Non-Streaming | Streaming |
|---------------|-----------|
| `GetResponseAsync` | `GetStreamingResponseAsync` |
| Returns `ChatResponse` | Returns `IAsyncEnumerable<ChatResponseUpdate>` |
| Wait for full response | Process each token as it arrives |
| Simpler code | Better user experience |

### Complete Streaming Example

Here's a full streaming chat application:

```csharp
using Microsoft.Extensions.AI;

IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(githubToken))
        .AsIChatClient("gpt-4o-mini");

List<ChatMessage> chatHistory = [];

while (true)
{
    Console.Write("Q: ");
    chatHistory.Add(new(ChatRole.User, Console.ReadLine()));

    List<ChatResponseUpdate> updates = [];
    await foreach (ChatResponseUpdate update in
        client.GetStreamingResponseAsync(chatHistory))
    {
        Console.Write(update.Text);
        updates.Add(update);
    }
    Console.WriteLine();

    // Add the streamed response to history
    chatHistory.AddMessages(updates);
}
```

**Key points:**

1. `GetStreamingResponseAsync` returns an `IAsyncEnumerable<ChatResponseUpdate>`
2. Each `update` contains a piece of the response (often a single word or token)
3. Collect all updates to add to conversation history
4. Use `AddMessages(updates)` to combine updates into a complete response

### Converting Updates to a Full Response

Sometimes you need both streaming display and a complete response object. Use `ToChatResponse`:

```csharp
List<ChatResponseUpdate> updates = [];
await foreach (var update in client.GetStreamingResponseAsync(prompt))
{
    Console.Write(update.Text);
    updates.Add(update);
}

// Convert collected updates to a single ChatResponse
ChatResponse fullResponse = updates.ToChatResponse();
Console.WriteLine($"\n\nFull response had {fullResponse.Messages.Count} messages");
```

---

## Part 2: Structured Output

Text responses are flexible, but sometimes you need **structured data** that your code can easily process. For example:

- Extracting data into specific fields
- Getting responses as JSON objects
- Classification into predefined categories

Microsoft.Extensions.AI provides **structured output** support that returns strongly-typed objects instead of plain text.

### Basic Structured Output

Let's analyze sentiment and get a typed result:

**Step 1: Define your output type**

```csharp
public enum Sentiment
{
    Positive,
    Negative,
    Neutral
}
```

**Step 2: Request structured output**

```csharp
string review = "I'm happy with the product!";
var response = await chatClient.GetResponseAsync<Sentiment>(
    $"What's the sentiment of this review? {review}");

Console.WriteLine($"Sentiment: {response.Result}");
// Output: Sentiment: Positive
```

The `GetResponseAsync<T>` extension method instructs the model to return a response matching your type, and automatically deserializes it.

### Analyzing Multiple Items

You can process collections efficiently:

```csharp
string[] reviews = [
    "Best purchase ever!",
    "Returned it immediately.",
    "Hello",
    "It works as advertised.",
    "The packaging was damaged but otherwise okay."
];

foreach (var review in reviews)
{
    var response = await chatClient.GetResponseAsync<Sentiment>(
        $"What's the sentiment of this review? {review}");
    Console.WriteLine($"Review: {review} | Sentiment: {response.Result}");
}
```

**Output:**

```
Review: Best purchase ever! | Sentiment: Positive
Review: Returned it immediately. | Sentiment: Negative
Review: Hello | Sentiment: Neutral
Review: It works as advertised. | Sentiment: Neutral
Review: The packaging was damaged but otherwise okay. | Sentiment: Neutral
```

### Complex Structured Output with Records

For more complex responses, define a record type:

```csharp
record SentimentAnalysis(
    string ResponseText,
    Sentiment ReviewSentiment,
    double ConfidenceScore,
    string[] KeyPhrases
);
```

Then request it:

```csharp
var review = "This product exceeded my expectations in every way!";
var response = await chatClient.GetResponseAsync<SentimentAnalysis>(
    $"Analyze this review: {review}");

Console.WriteLine($"Text: {response.Result.ResponseText}");
Console.WriteLine($"Sentiment: {response.Result.ReviewSentiment}");
Console.WriteLine($"Confidence: {response.Result.ConfidenceScore}");
Console.WriteLine($"Key phrases: {string.Join(", ", response.Result.KeyPhrases)}");
```

### Practical Example: Extracting Contact Information

```csharp
record ContactInfo(
    string Name,
    string? Email,
    string? Phone,
    string? Company
);

var text = @"
    Hi, I'm Sarah Johnson from Contoso Ltd. 
    You can reach me at sarah.johnson@contoso.com 
    or call my office at 555-0123.
";

var response = await chatClient.GetResponseAsync<ContactInfo>(
    $"Extract contact information from this text: {text}");

var contact = response.Result;
Console.WriteLine($"Name: {contact.Name}");
Console.WriteLine($"Email: {contact.Email}");
Console.WriteLine($"Phone: {contact.Phone}");
Console.WriteLine($"Company: {contact.Company}");
```

**Output:**

```
Name: Sarah Johnson
Email: sarah.johnson@contoso.com
Phone: 555-0123
Company: Contoso Ltd
```

### Lists and Collections

You can also request lists of structured items:

```csharp
record ActionItem(
    string Task,
    string? Assignee,
    string? DueDate,
    string Priority
);

var meetingNotes = @"
    Discussed Q1 roadmap. John will prepare the budget by Friday (high priority).
    Sarah needs to review the marketing plan next week (medium priority).
    Team should update documentation ongoing (low priority).
";

var response = await chatClient.GetResponseAsync<List<ActionItem>>(
    $"Extract all action items from these meeting notes: {meetingNotes}");

foreach (var item in response.Result)
{
    Console.WriteLine($"- {item.Task} | {item.Assignee} | {item.Priority}");
}
```

---

## Part 3: Combining Streaming and Structured Output

For scenarios where you want real-time feedback but also need structured data, you can:

1. Stream for user feedback
2. Then make a structured output call for data extraction

```csharp
// First, stream the conversational response
Console.WriteLine("AI is analyzing...");
string fullText = "";
await foreach (var update in client.GetStreamingResponseAsync(userQuery))
{
    Console.Write(update.Text);
    fullText += update.Text;
}

// Then extract structured data from the response
var structured = await client.GetResponseAsync<AnalysisResult>(
    $"Extract key data from: {fullText}");
```

---

## ChatOptions for Fine-Tuning Responses

Both streaming and structured output support `ChatOptions` for additional control:

```csharp
var options = new ChatOptions
{
    Temperature = 0.2f,  // Lower = more deterministic
    MaxOutputTokens = 500
};

await foreach (var update in client.GetStreamingResponseAsync(prompt, options))
{
    Console.Write(update.Text);
}
```

Common `ChatOptions` properties:

| Property | Description |
|----------|-------------|
| `Temperature` | Controls randomness (0-2). Lower = more focused. |
| `MaxOutputTokens` | Limits response length |
| `TopP` | Alternative to temperature for controlling randomness |
| `ModelId` | Override the default model |

---

## Let's Review: What You Learned

| Concept | Key Takeaway |
|---------|-------------|
| **Streaming** | Display tokens as they arrive for better UX |
| **IAsyncEnumerable** | C# pattern for async streams of data |
| **ToChatResponse** | Convert stream updates into a complete response |
| **Structured Output** | Get typed objects instead of plain text |
| **Record Types** | Perfect for defining structured output schemas |

### Quick Self-Check

Can you answer these questions?

1. When should you use streaming instead of a regular response?
2. How do you add streamed responses to conversation history?
3. What's the advantage of structured output over parsing text responses?

---

## Sample Code Reference

| Sample | Description |
|--------|-------------|
| [BasicChat-01MEAI](../samples/CoreSamples/BasicChat-01MEAI/) | Basic chat with GitHub Models |
| [BasicChat-10ConversationHistory](../samples/CoreSamples/BasicChat-10ConversationHistory/) | Conversation history management |

---

## Additional Resources

- [Request Structured Output](https://learn.microsoft.com/dotnet/ai/quickstarts/structured-output)
- [IChatClient Streaming Documentation](https://learn.microsoft.com/dotnet/ai/ichatclient#request-a-streaming-chat-response)

---

## Up Next

Now that you can stream responses and get structured data, let's extend AI capabilities with your own code:

[Continue to Part 3: Function Calling](./03-function-calling.md)
