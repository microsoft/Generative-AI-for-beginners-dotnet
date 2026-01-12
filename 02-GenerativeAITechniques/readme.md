# Generative AI Techniques for .NET Developers

In this lesson you'll learn practical skills for building AI-enabled .NET applications. Concepts include text completions, chat conversations, streaming responses, structured output, function calling, and building middleware pipelines.

---

> **DRAFT NOTE:** Video thumbnail needed. Follow naming pattern: `LIM_GAN_02_thumb_w480.png`

<!-- Uncomment when video thumbnail is available:
[![Generative AI Techniques](./images/LIM_GAN_02_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson2-techniques)

_Click the image to watch the video_
-->

---

## What You Will Learn in This Lesson

- Text completions and chat conversations
- Managing conversation history with chat roles
- Streaming responses for real-time output
- Getting structured output from AI models
- Function calling to extend AI with your code
- Building middleware pipelines with caching and telemetry

For this lesson, we will subdivide the content into the following sections:

- [Text Completions and Chat Conversations](./01-text-completions-chat.md)
- [Streaming and Structured Output](./02-streaming-structured-output.md)
- [Function Calling](./03-function-calling.md)
- [Middleware Pipelines](./04-middleware-pipeline.md)

[Go to part 1: Text Completions and Chat Conversations](./01-text-completions-chat.md)

---

## Sample Code Organization

All code samples for this lesson are located in the **[`samples/CoreSamples/`](../samples/CoreSamples/)** directory. This includes:

- **Chat & Completions**: `BasicChat-01MEAI`, `BasicChat-02SK`, `BasicChat-03Ollama`, etc.
- **Conversation History**: `BasicChat-10ConversationHistory`
- **Function Calling**: `MEAIFunctions`, `MEAIFunctionsAzureOpenAI`, `MEAIFunctionsOllama`

Each lesson document links directly to the relevant samples in this centralized location.

---

## Next Steps

Once you complete all parts of this lesson, you'll be ready to tackle more advanced patterns in Lesson 3:

- **Retrieval-Augmented Generation (RAG)**: Ground AI responses in your own documents
- **Semantic Search**: Search by meaning, not just keywords
- **Vision and Audio**: Work with images and sound

[Continue to Lesson 3: AI Patterns and Applications](../03-CoreGenerativeAITechniques/readme.md)

---

## Additional Resources

- [Microsoft.Extensions.AI Documentation](https://learn.microsoft.com/dotnet/ai/ai-extensions)
- [IChatClient Interface Guide](https://learn.microsoft.com/dotnet/ai/ichatclient)
- [Build an AI Chat App with .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/build-chat-app)
- [Request Structured Output](https://learn.microsoft.com/dotnet/ai/quickstarts/structured-output)
