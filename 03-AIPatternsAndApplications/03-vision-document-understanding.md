# Vision and Document Understanding

In this lesson, you'll learn how to use AI to analyze images, extract information from documents, and build applications that can "see."

---

[![Vision and Document Understanding](./images/LIM_GAN_06_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson3-vision)

_Click the image to watch the video_

---

## Beyond Text

Everything we've done so far has been text-based. But the real world isn't just text. It's receipts, invoices, diagrams, photos, and documents that combine words with visual elements.

Modern AI models have evolved to understand:

- **Images** - Photos, diagrams, screenshots, charts, product images
- **Documents** - PDFs, scanned papers, handwritten forms, contracts
- **Mixed content** - Pages combining text, tables, images, and graphics

This capability is called **multimodal AI**, models that process multiple types of content simultaneously. Instead of describing an image in text, you can simply show it to the AI and ask questions about it.

> **Why this matters:** Many business processes still rely on visual documents. Expense reports need receipt scanning. Customer service needs to understand uploaded screenshots. Legal teams need to analyze scanned contracts. Vision AI automates these workflows.

**Learn more:** [Use vision-enabled chat models](https://learn.microsoft.com/azure/ai-services/openai/how-to/gpt-with-vision) on Microsoft Learn.

---

## Part 1: Image Recognition with MEAI

Microsoft.Extensions.AI handles images through the `DataContent` class. This class wraps binary data (like image bytes) along with a MIME type, allowing you to send images directly to vision-capable models.

The pattern is simple: instead of sending just text in your chat message, you include both `TextContent` (your question) and `DataContent` (the image). The model sees both and can reason about the image while responding to your question.

> **Note:** Images consume tokens just like text. Higher resolution images use more tokens. See [Understanding tokens](https://learn.microsoft.com/dotnet/ai/conceptual/understanding-tokens) to learn how this affects pricing and limits.

### Basic Image Analysis

Here's the fundamental pattern. Load an image, wrap it in `DataContent`, and ask a question:

```csharp
using Microsoft.Extensions.AI;
using Azure;
using Azure.AI.Inference;

// Create a chat client with a vision-capable model
IChatClient chatClient = new ChatCompletionsClient(
    endpoint: new Uri("https://models.github.ai/inference"),
    new AzureKeyCredential(githubToken))
    .AsIChatClient("gpt-4o");  // Must be a vision-capable model

// Load an image from file
var imageBytes = await File.ReadAllBytesAsync("product-photo.jpg");
var image = new DataContent(imageBytes, "image/jpeg");

// Ask about the image
var messages = new List<ChatMessage>
{
    new(ChatRole.User, new AIContent[]
    {
        new TextContent("What product is shown in this image?"),
        image
    })
};

var response = await chatClient.GetResponseAsync(messages);
Console.WriteLine(response.Text);
```

### Describing Images in Detail

For richer descriptions, use a system prompt to guide the model's focus. This example instructs the AI to be thorough, noting specific visual elements:

```csharp
var messages = new List<ChatMessage>
{
    new(ChatRole.System, 
        "You are an image analyst. Describe images in detail, noting objects, colors, text, and context."),
    
    new(ChatRole.User, new AIContent[]
    {
        new TextContent("Describe this image in detail."),
        new DataContent(imageBytes, "image/png")
    })
};

var description = await chatClient.GetResponseAsync(messages);
```

### Comparing Images

You can send multiple images in the same message. The model sees all of them and can compare, contrast, or analyze relationships between them. This is useful for before/after comparisons, A/B testing visuals, or verifying changes:

```csharp
var image1 = new DataContent(await File.ReadAllBytesAsync("before.jpg"), "image/jpeg");
var image2 = new DataContent(await File.ReadAllBytesAsync("after.jpg"), "image/jpeg");

var messages = new List<ChatMessage>
{
    new(ChatRole.User, new AIContent[]
    {
        new TextContent("Compare these two images. What changed?"),
        image1,
        image2
    })
};

var comparison = await chatClient.GetResponseAsync(messages);
```

### Loading Images from URLs

You don't always need to load image bytes from disk. If the image is already hosted online, you can pass a URL directly. The model will fetch and analyze the image:

```csharp
// Pass a URL instead of loading bytes - the model fetches the image
var imageUrl = "https://example.com/product-image.jpg";
var imageFromUrl = new DataContent(new Uri(imageUrl));

var messages = new List<ChatMessage>
{
    new(ChatRole.User, new AIContent[]
    {
        new TextContent("What brand is this product?"),
        imageFromUrl
    })
};
```

**Learn more:** [Microsoft.Extensions.AI libraries](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai) covers the DataContent class and other MEAI abstractions.

---

## Part 2: Practical Vision Applications

Now let's see vision AI solving real business problems. These examples show how to extract structured data from visual documents, turning images into actionable information.

### Receipt/Invoice Processing

Expense management systems need to extract vendor, date, items, and totals from receipts. Vision AI can read the receipt and return structured JSON, eliminating manual data entry:

```csharp
var receiptImage = new DataContent(
    await File.ReadAllBytesAsync("receipt.jpg"), 
    "image/jpeg");

var messages = new List<ChatMessage>
{
    new(ChatRole.System, """
        You are an expense processing assistant. Extract information 
        from receipts and return structured data.
        """),
    
    new(ChatRole.User, new AIContent[]
    {
        new TextContent("""
            Extract the following from this receipt:
            - Vendor name
            - Date
            - Total amount
            - List of items with prices
            
            Return as JSON.
            """),
        receiptImage
    })
};

var response = await chatClient.GetResponseAsync(messages);

// Parse the JSON response
var receiptData = JsonSerializer.Deserialize<ReceiptData>(response.Text);
```

### Form Analysis

Paper forms, whether scanned or photographed, can be processed automatically. The AI identifies form fields, reads handwritten or typed values, and even understands checkbox states:

```csharp
var formImage = new DataContent(
    await File.ReadAllBytesAsync("application-form.png"), 
    "image/png");

var messages = new List<ChatMessage>
{
    new(ChatRole.User, new AIContent[]
    {
        new TextContent("""
            This is an application form. Extract all filled-in fields 
            and their values. For checkboxes, indicate if they are 
            checked or unchecked.
            """),
        formImage
    })
};
```

### Diagram Understanding

Technical diagrams, flowcharts, and architecture drawings contain complex relationships. Vision AI can interpret these visuals, identify components, and explain how they connect:

```csharp
var diagramImage = new DataContent(
    await File.ReadAllBytesAsync("architecture-diagram.png"), 
    "image/png");

var messages = new List<ChatMessage>
{
    new(ChatRole.System, 
        "You are a technical analyst who explains system architectures."),
    
    new(ChatRole.User, new AIContent[]
    {
        new TextContent("Explain this system architecture diagram. What are the main components and how do they interact?"),
        diagramImage
    })
};
```

---

## Part 3: Document Processing

Single images are straightforward, but real documents are often multi-page PDFs. Vision models work with images, not PDFs directly, so you need to:

1. **Convert each PDF page to an image** (PNG or JPEG)
2. **Process each page through the vision model** 
3. **Combine the results** into a complete document analysis

This page-by-page approach works well because it respects context boundaries (each page is self-contained) while allowing you to analyze the full document.

### PDF Processing Strategy

Here's the pattern: convert to images, process each page, collect results:

```csharp
using System.Drawing;  // For PDF-to-image conversion
// Or use a library like PdfiumViewer, ImageMagick, etc.

async Task<string> ProcessPdfAsync(string pdfPath)
{
    var results = new StringBuilder();
    
    // Convert PDF pages to images
    var pageImages = ConvertPdfToImages(pdfPath);
    
    foreach (var (pageImage, pageNumber) in pageImages.Select((img, i) => (img, i + 1)))
    {
        var image = new DataContent(pageImage, "image/png");
        
        var messages = new List<ChatMessage>
        {
            new(ChatRole.User, new AIContent[]
            {
                new TextContent($"Extract all text and describe any diagrams on page {pageNumber}."),
                image
            })
        };
        
        var response = await chatClient.GetResponseAsync(messages);
        results.AppendLine($"## Page {pageNumber}");
        results.AppendLine(response.Text);
        results.AppendLine();
    }
    
    return results.ToString();
}
```

### Document Summarization

```csharp
async Task<string> SummarizeDocumentAsync(string pdfPath)
{
    // First, extract content from all pages
    var fullContent = await ProcessPdfAsync(pdfPath);
    
    // Then summarize
    var messages = new List<ChatMessage>
    {
        new(ChatRole.System, 
            "You are a document analyst. Provide concise summaries."),
        
        new(ChatRole.User, $"""
            Summarize this document in 3-5 bullet points:
            
            {fullContent}
            """)
    };
    
    var summary = await chatClient.GetResponseAsync(messages);
    return summary.Text;
}
```

**Learn more:** [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/) provides production-grade document processing for complex enterprise workflows.

---

## Part 4: Vision with Local Models

Cloud vision APIs are convenient, but sometimes you need to process images locally for privacy, offline operation, or cost control. Ollama supports vision-capable models that run entirely on your machine.

The best part? The MEAI abstraction means your code stays the same. Just swap the chat client:

```csharp
using OllamaSharp;

// Vision-capable local models
var client = new OllamaApiClient("http://localhost:11434");
IChatClient chatClient = client.AsIChatClient("llava");  // or "llama3.2-vision"

// Same code works!
var imageBytes = await File.ReadAllBytesAsync("photo.jpg");
var image = new DataContent(imageBytes, "image/jpeg");

var messages = new List<ChatMessage>
{
    new(ChatRole.User, new AIContent[]
    {
        new TextContent("What's in this image?"),
        image
    })
};

var response = await chatClient.GetResponseAsync(messages);
Console.WriteLine(response.Text);
```

**Local Vision Models:**

| Model | Size | Good For |
|-------|------|----------|
| llava | 4GB | General image understanding |
| llama3.2-vision | 11GB | High-quality vision analysis |
| bakllava | 4GB | Fast general analysis |

---

## Part 5: Building a Document Q&A System

The most powerful pattern combines vision with conversation history. Instead of one-off image analysis, you maintain context across multiple questions about the same document.

This example creates a document assistant that:
- **Loads a document** and provides an initial summary
- **Answers follow-up questions** while maintaining conversation context
- **References specific parts** of the document in responses

```csharp
public class DocumentQA
{
    private readonly IChatClient _chatClient;
    private readonly List<ChatMessage> _conversationHistory;
    private byte[]? _documentImage;

    public DocumentQA(IChatClient chatClient)
    {
        _chatClient = chatClient;
        _conversationHistory = new List<ChatMessage>
        {
            new(ChatRole.System, """
                You are a document analyst. Help users understand and 
                extract information from documents. Always reference 
                specific parts of the document in your answers.
                """)
        };
    }

    public async Task LoadDocumentAsync(string imagePath)
    {
        _documentImage = await File.ReadAllBytesAsync(imagePath);
        
        // Initial analysis
        _conversationHistory.Add(new ChatMessage(ChatRole.User, new AIContent[]
        {
            new TextContent("I'm uploading a document. Please briefly describe what it contains."),
            new DataContent(_documentImage, "image/png")
        }));
        
        var response = await _chatClient.GetResponseAsync(_conversationHistory);
        _conversationHistory.Add(response.ToAssistantMessage());
        
        Console.WriteLine($"Document loaded: {response.Text}");
    }

    public async Task<string> AskAsync(string question)
    {
        // Include image reference with each question for context
        _conversationHistory.Add(new ChatMessage(ChatRole.User, new AIContent[]
        {
            new TextContent(question),
            new DataContent(_documentImage!, "image/png")
        }));
        
        var response = await _chatClient.GetResponseAsync(_conversationHistory);
        _conversationHistory.Add(response.ToAssistantMessage());
        
        return response.Text;
    }
}

// Usage
var docQA = new DocumentQA(chatClient);
await docQA.LoadDocumentAsync("contract.png");

Console.WriteLine(await docQA.AskAsync("What is the effective date?"));
Console.WriteLine(await docQA.AskAsync("List all parties mentioned."));
Console.WriteLine(await docQA.AskAsync("What are the termination conditions?"));
```

---

## Part 6: Vision Best Practices

Vision AI quality depends heavily on input quality. Poor images lead to poor results or hallucinated content. Follow these guidelines for reliable document processing.

### Image Quality Tips

| Tip | Reason |
|-----|--------|
| Use high resolution | Models work better with clear images |
| Avoid heavy compression | JPEG artifacts can confuse analysis |
| Good lighting | Shadows hide important details |
| Straight alignment | Tilted documents are harder to read |

### Performance Considerations

```csharp
// Resize large images before sending
byte[] ResizeImage(byte[] imageBytes, int maxWidth = 1920)
{
    using var ms = new MemoryStream(imageBytes);
    using var image = Image.Load(ms);
    
    if (image.Width > maxWidth)
    {
        var ratio = maxWidth / (float)image.Width;
        var newHeight = (int)(image.Height * ratio);
        image.Mutate(x => x.Resize(maxWidth, newHeight));
    }
    
    using var output = new MemoryStream();
    image.SaveAsJpeg(output);
    return output.ToArray();
}
```

### Handling Sensitive Documents

```csharp
// Never log or store sensitive document content
var messages = new List<ChatMessage>
{
    new(ChatRole.System, """
        Process this document but do not retain any personal 
        information. Mask SSNs, credit card numbers, and 
        other sensitive data in your response.
        """),
    // ...
};
```

---

## Let's Review: What You Learned

| Concept | Key Takeaway |
|---------|-------------|
| **DataContent** | Class for sending images to AI models |
| **Multimodal** | Models that understand text + images |
| **Document Processing** | Convert PDFs to images, process page by page |
| **Vision + Chat** | Build document Q&A with conversation history |
| **Local Vision** | Run analysis locally with llava/llama3.2-vision |

### Quick Self-Check

1. What's the difference between text-only and multimodal AI?
2. Why do we convert PDFs to images for processing?
3. When would you use local vision models vs. cloud?

---

## Sample Code Reference

| Sample | Description |
|--------|-------------|
| [Vision-01MEAI-GitHubModels](../samples/CoreSamples/Vision-01MEAI-GitHubModels/) | Vision with GitHub Models |
| [Vision-02MEAI-Ollama](../samples/CoreSamples/Vision-02MEAI-Ollama/) | Local vision with Ollama |
| [Vision-03MEAI-AOAI](../samples/CoreSamples/Vision-03MEAI-AOAI/) | Vision with Azure OpenAI |
| [Vision-04MEAI-AOAI-Spectre](../samples/CoreSamples/Vision-04MEAI-AOAI-Spectre/) | Vision with Spectre Console output |
| [OpenAI-FileProcessing-Pdf-01](../samples/CoreSamples/OpenAI-FileProcessing-Pdf-01/) | PDF document processing |

---

## Additional Resources

- [Use vision-enabled chat models](https://learn.microsoft.com/azure/ai-services/openai/how-to/gpt-with-vision) - Complete guide to GPT-4o vision capabilities
- [Understanding tokens](https://learn.microsoft.com/dotnet/ai/conceptual/understanding-tokens) - How images affect token usage and pricing
- [Microsoft.Extensions.AI libraries](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai) - The MEAI abstraction layer used in this lesson
- [Azure AI Document Intelligence](https://learn.microsoft.com/azure/ai-services/document-intelligence/) - Production-grade document processing service
- [Develop generative AI apps with Azure OpenAI](https://learn.microsoft.com/training/paths/develop-ai-agents-azure-open-ai-semantic-kernel-sdk) - Microsoft Learn training path

---

## Up Next

Now you know embeddings, RAG, and vision. Let's put them together to build powerful combined solutions:

[Continue to Part 4: Combining Patterns →](./04-combining-patterns.md)
