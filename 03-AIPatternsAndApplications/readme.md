# Lesson 3: AI Patterns and Applications in .NET

In this lesson you'll learn how to apply AI patterns to solve real problems. This is where concepts become solutions. You'll build applications that understand meaning, ground responses in your data, and process documents intelligently.

---

[![AI Patterns and Applications](./images/LIM_GAN_07_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson3-patterns)

_Click the image to watch the video_

---

## What You'll Learn

- How embeddings represent meaning as numbers
- Build semantic search that understands intent, not just keywords
- Implement Retrieval-Augmented Generation (RAG) to ground AI in your data
- Create applications that process and understand documents and images
- Know when to use each pattern and how to combine them

---

## Lesson Structure

This lesson is divided into four parts:

### [Part 1: Embeddings and Semantic Search](./01-embeddings-semantic-search.md)
Understand how AI represents meaning as vectors and build search that finds by intent.

### [Part 2: Retrieval-Augmented Generation (RAG)](./02-retrieval-augmented-generation.md)
Ground AI responses in your own documents and data.

### [Part 3: Vision and Document Understanding](./03-vision-document-understanding.md)
Process images, PDFs, and visual content with multimodal AI.

### [Part 4: Combining Patterns](./04-combining-patterns.md)
Build sophisticated applications that combine multiple patterns.

---

## Why Patterns Matter

In the previous lesson, you learned the techniques: chat, streaming, function calling, middleware. But techniques alone don't solve problems.

**Patterns** are proven combinations of techniques that solve specific types of problems:

| Pattern | Problem It Solves |
|---------|------------------|
| **Semantic Search** | "Find things by meaning, not keywords" |
| **RAG** | "Answer questions using my specific data" |
| **Vision Processing** | "Understand and extract information from images" |
| **Document Understanding** | "Process and analyze document content" |

This lesson teaches you to recognize problems and apply the right pattern.

---

## Sample Code Reference

All code samples for this lesson are located in the **[`samples/CoreSamples/`](../samples/CoreSamples/)** directory:

| Category | Samples | Description |
|----------|---------|-------------|
| **Embeddings & RAG** | [RAGSimple-02MEAIVectorsMemory](../samples/CoreSamples/RAGSimple-02MEAIVectorsMemory/) | In-memory vector store |
| | [RAGSimple-03MEAIVectorsAISearch](../samples/CoreSamples/RAGSimple-03MEAIVectorsAISearch/) | Azure AI Search |
| | [RAGSimple-04MEAIVectorsQdrant](../samples/CoreSamples/RAGSimple-04MEAIVectorsQdrant/) | Qdrant vector store |
| **Vision** | [Vision-01MEAI-GitHubModels](../samples/CoreSamples/Vision-01MEAI-GitHubModels/) | Vision with GitHub Models |
| | [Vision-02MEAI-Ollama](../samples/CoreSamples/Vision-02MEAI-Ollama/) | Local vision with Ollama |
| | [Vision-03MEAI-AOAI](../samples/CoreSamples/Vision-03MEAI-AOAI/) | Vision with Azure OpenAI |
| **Documents** | [OpenAI-FileProcessing-Pdf-01](../samples/CoreSamples/OpenAI-FileProcessing-Pdf-01/) | PDF document processing |

Each lesson document links directly to the relevant samples.

---

## Let's Begin

Start with understanding how AI represents meaning:

[Continue to Part 1: Embeddings and Semantic Search →](./01-embeddings-semantic-search.md)

---

## Next Steps

Once you complete all parts of this lesson, you'll be ready for AI Agents in Lesson 4:

- Building autonomous agents that make decisions
- Multi-agent orchestration
- Agent tools and plugins

[Continue to Lesson 4: AI Agents →](../04-AgentsWithMAF/readme.md)

---

## Additional Resources

- [Microsoft.Extensions.VectorData Documentation](https://learn.microsoft.com/dotnet/ai/conceptual/vector-databases): Working with vector databases in .NET
- [Build a Vector Search App](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-data): End-to-end quickstart for semantic search
- [Embeddings Explained](https://learn.microsoft.com/dotnet/ai/conceptual/embeddings): How AI represents meaning as numbers
