# AI Patterns and Applications in .NET

In this lesson you'll learn how to apply AI patterns to solve real problems. This is where concepts become solutions - you'll build applications that understand meaning, ground responses in your data, and process documents intelligently.

---

> **Video coming soon**

<!-- Uncomment when video thumbnail is available:
[![AI Patterns and Applications](./images/LIM_GAN_07_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson3-patterns)

_Click the image to watch the video_
-->

---

## What You Will Learn in This Lesson

- How embeddings represent meaning as numbers
- Build semantic search that understands intent, not just keywords
- Implement Retrieval-Augmented Generation (RAG) to ground AI in your data
- Create applications that process and understand documents and images
- Know when to use each pattern and how to combine them

For this lesson, we will subdivide the content into the following sections:

- [Embeddings and Semantic Search](./01-embeddings-semantic-search.md)
- [Retrieval-Augmented Generation (RAG)](./02-retrieval-augmented-generation.md)
- [Vision and Document Understanding](./03-vision-document-understanding.md)
- [Combining Patterns](./04-combining-patterns.md)

[Go to part 1: Embeddings and Semantic Search](./01-embeddings-semantic-search.md)

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

## Sample Code Organization

All code samples for this lesson are located in the **[`samples/CoreSamples/`](../samples/CoreSamples/)** directory. This includes:

- **RAG Examples**: `RAGSimple-01SK`, `RAGSimple-02MEAIVectorsMemory`, `RAGSimple-03MEAIVectorsAISearch`, etc.
- **Vision Examples**: `Vision-01MEAI-GitHubModels`, `Vision-02MEAI-Ollama`, `Vision-03MEAI-AOAI`
- **Document Processing**: `OpenAI-FileProcessing-Pdf-01`

Each lesson document links directly to the relevant samples.

---

## Next Steps

Once you complete all parts of this lesson, you'll be ready for AI Agents in Lesson 4:

- Building autonomous agents that make decisions
- Multi-agent orchestration
- Agent tools and plugins

[Continue to Lesson 4: AI Agents](../04-AIAgents/readme.md)

---

## Additional Resources

- [Microsoft.Extensions.VectorData Documentation](https://learn.microsoft.com/dotnet/ai/conceptual/vector-databases)
- [Build a Vector Search App](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-ai-chat-with-data)
- [Embeddings Explained](https://learn.microsoft.com/dotnet/ai/conceptual/embeddings)
