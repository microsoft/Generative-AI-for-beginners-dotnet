# Getting Started with AI Development Tools

Refresh your generative AI knowledge and understand the .NET tooling available to help you to develop generative AI applications.

---

[![Introduction to Generative AI](http://img.youtube.com/vi/SZvE_meBdvg/0.jpg)](http://www.youtube.com/watch?v=SZvE_meBdvg)

_⬆️Click the image to watch the video⬆️_

## What you'll learn in this lesson:

- 🌟 Understand fundamental concepts of generative AI and their applications
- 🔍 Explore the .NET tooling for AI development including MEAI, Semantic Kernel, and Azure OpenAI

## Generative AI Fundamentals for .NET

Before we dive in to some code, let's take a  minute to review some generative AI (GenAI) concepts. In this lesson, **Generative AI Fundamentals for .NET**, we'll refresh some fundamental GenAI concepts so you can understand why certain things are done like they are. And we'll introduce the tooling and SDKs you'll use to build apps, like **MEAI** (Microsoft.Extensions.AI), **Semantic Kernel**, and the **AI Toolkit Extension for VS Code**.

### A quick refresh on Generative AI concepts

Generative AI is a type of artificial intelligence that creates new content, such as text, images, or code, based on patterns and relationships learned from data. Generative AI models can generate human-like responses, understand context, and sometimes even create content that seems human-like.

As you develop your .NET AI applications, you'll work with **generative AI models** to create content. Some capabilities of generative AI models include:

- **Text Generation**: Crafting human-like text for chatbots, content, and text completion.
- **Image Generation and Analysis**: Producing realistic images, enhancing photos, and detecting objects.
- **Code Generation**: Writing code snippets or scripts.

There are specific types of models that are optimized for different tasks. For example, **Small Language Models (SLMs)** are ideal for text generation, while **Large Language Models (LLMs)** are more suitable for complex tasks like code generation or image analysis. And from there different companies and groups develop models, like Microsoft, OpenAI, or Anthropic. The specific one you use will depend on your use case and the capabilities you need.

Of course, the responses from these models are not perfect all the time. You're probably heard about models "hallucinating" or generating incorrect information in an authoritative manner. But you can help guide the model to generate better responses by providing them with clear instructions and context. This is where **prompt engineering** comes in.

#### Prompt engineering review

Prompt engineering is the practice of designing effective inputs to guide AI models toward desired outputs. It involves:

- **Clarity**: Making instructions clear and unambiguous.
- **Context**: Providing necessary background information.
- **Constraints**: Specifying any limitations or formats.

Some best practices for prompt engineering include, prompt design, clear instructions, task breakdown, one shot and few shot learning, and prompt tuning. Plus, trying and testing different prompts to see what works best for your specific use case.

And it's important to note there are different types of prompts when developing applications. For example, you'll be responsbile for setting **system prompts** that set the base rules and context for the model's response. The data the user of your application feeds into the model are known as **user prompts**. And **assistant prompts** are the responses the model generates based on the system and user prompts.

> 🧑‍🏫 **Learn more**: Learn more about prompt engineering in [Prompt Engineering chapter of GenAI for Beginners course](https://github.com/microsoft/generative-ai-for-beginners/tree/main/04-prompt-engineering-fundamentals)

#### Tokens, embeddings, and agents - oh my!

When working with generative AI models, you'll encounter terms like **tokens**, **embeddings**, and **agents**. Here's a quick overview of these concepts:

- **Tokens**: Tokens are the smallest unit of text in a model. They can be words, characters, or subwords. Tokens are used to represent text data in a format that the model can understand.
- **Embeddings**: Embeddings are vector representations of tokens. They capture the semantic meaning of words and phrases, allowing models to understand relationships between words and generate contextually relevant responses.
- **Vector databases**: Vector databases are collections of embeddings that can be used to compare and analyze text data. They enable models to generate responses based on the context of the input data.
- **Agents**: Agents are AI components that interact with models to generate responses. They can be chatbots, virtual assistants, or other applications that use generative AI models to create content.

When developing .NET AI applications, you'll work with tokens, embeddings, and agents to create chatbots, content generators, and other AI-powered applications. Understanding these concepts will help you build more effective and efficient AI applications.

### AI Development Tools and Libraries for .NET

.NET offers a range of tooling for AI development. Lets take a minute to understand some of the tools and libraries available.

#### Microsoft.Extensions.AI (MEAI)

The Microsoft.Extensions.AI (MEAI) library provides unified abstractions and middleware to simplify the integration of AI services into .NET applications.

By providing a consistent API, MEAI enables developers to interact with different AI services, such as small and large language models, embeddings, and even middleware through a common interface. This lowers the friction it takes to build an .NET AI application as you'll be developing against the same API for different services.

For example, here's the interface you would use to create a chat client with MEAI regardless of the AI service you're using:

```csharp
public interface IChatClient : IDisposable 
{ 
    Task<ChatCompletion> CompleteAsync(...); 
    IAsyncEnumerable<StreamingChatCompletionUpdate> CompleteStreamingAsync(...); 
    ChatClientMetadata Metadata { get; } 
    TService? GetService<TService>(object? key = null) where TService : class; 
}
```

This way when using MEAI to build a chat application, you'll develop against the same API surface to get a chat completion or stream the completion, get metadata, or access the underlying AI service. This makes it easier to switch out AI services or add new ones as needed.

Additionally, the library supports middleware components for functionalities like logging, caching and telemetry making it easier to develop robust AI applications.

![*Figure: Microsoft.Extensions.AI (MEAI) library.*](./images/meai-architecture-diagram.png)

Using an unified API, MEAI allows developers to work with different AI services, such as Azure AI Inference, Ollama, and OpenAI, in a consistent manner. This simplifies the integration of AI models into .NET applications, adding flexibility for developers to choose the best AI services for their projects and specific requirements.

> 🏎️ **Quick start**: For a quick start with MEAI, [check out the blog post](https://devblogs.microsoft.com/dotnet/introducing-microsoft-extensions-ai-preview/).
>
> 📖 **Docs**: Learn more about Microsoft.Extensions.AI (MEAI) in the [MEAI documentation](https://learn.microsoft.com/dotnet/ai/ai-extensions)
 

#### Semantic Kernel (SK)

Semantic Kernel is an open-source SDK that enables developers to integrate generative AI language models into their .NET applications. It provides abstractions for AI services and memory (vector) stores allowing creation of plugins that can be automatically orchestrated by AI. It even uses the OpenAPI standard enabling developers to create AI agents to interact with external APIs.

![*Figure: Semantic Kernel (SK) SDK.*](./images/semantic-kernel.png)

Semantic Kernel supports .NET, as well as other languages such as Java and Python, offering a plethora of connectors, functions and plugins for integration. Some of the key features of Semantic Kernel include:

- **Kernel Core**: Provides the core functionality for the Semantic Kernel, including connectors, functions, and plugins, to interact with AI services and models. The kernel is the heart of the Semantic Kernel, being available to services and plugins, retrieving them when needed, monitoring Agents, and being an active middleware for your application.

    For example, it can pick the best AI service for a specific task, build plus send the prompt to the service, and return the response to the application. Below, a diagram of the Kernel Core in action:

    ![*Figure: Semantic Kernel (SK) Kernel Core.*](./images/semantic-kernel-core.png)

- **AI Service Connectors**: Provides an abstraction layer to expose AI services to multiple providers, with a common and consistent interface, some examples like Chat Completion, Text to Image, Text to Speech, and Audio to Text.

- **Vector Store Connectors**: Exposes vector stores to multiple providers, via a common and consistent interface, allowing developers to work with embeddings, vectors, and other data representations.

- **Functions and Plugins**: Offers a range of functions and plugins for common AI tasks, such as function processing, Prompt Templating, Text Search, and more. Connecting this to the AI Service/Model, creating implementations for RAG, and agents, for example.

- **Prompt Templating**: Provides tools for prompt engineering, including prompt design, testing, and optimization, to enhance AI model performance and accuracy. Allowing developers to create and test prompts, and optimize them for specific tasks.

- **Filters**: Controls around when and how functions are run to improve security and responsible AI practices.

In Semantic Kernel, a full loop would look like the diagram below:

![*Figure: Semantic Kernel (SK) full loop.*](./images/semantic-kernel-full-loop.png)

> 📖 **Docs**: Learn more about Semantic Kernel in the [Semantic Kernel documentation](https://learn.microsoft.com/semantic-kernel/overview/)

## Conclusion

Generative AI offers a world of possibilities for developers, enabling them to create innovative applications that generate content, understand context, and provide human-like responses. The .NET ecosystem provides a range of tools and libraries to simplify AI development, making it easier to integrate AI capabilities into .NET applications.

## Next Steps

In the next chapters, we'll explore these scenarios in detail, providing hands-on examples, code snippets, and best practices to help you build real-world AI solutions using .NET!

Next up, we'll get your development environment setup! So you'll be ready to dive into the world of generative AI with .NET!

👉 [Set up your AI development environment](/02-SetupDevEnvironment/readme.md)
