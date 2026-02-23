# Core Generative AI Techniques

In this lesson you'll learn some practical skills for building AI-enabled .NET applications. Concepts include large language model completions and chat, Retrieval-Augmented Generation (RAG), Audio/Video analysis and even AI Agents.

---

#### What you'll learn in this lesson:

- 🌟 LLM completions and chat flows
- 🔗 Functions & plugins with LLMs
- 🔎 Retrieval-Augmented Generation (RAG)
- 👀 Vision-based AI approaches
- 🔊 Audio creation and transcription
- 🖼️ Image generation with DALL-E
- 🎬 Image and video generation with new models (gpt-image-1 and sora)
- 🧩 Agents & assistants
- 💻 Running models locally with AI Toolkit and Docker

For this lesson, we will subdivide the content into the following sections:

- [Chat, LLM completions, and function calling](./01-lm-completions-functions.md)
- [Retrieval-Augmented Generation (RAG)](./02-retrieval-augmented-generation.md)
- [Vision and audio AI applications](./03-vision-audio.md)
- [Agents](04-agents.md)
- [Image Generation with Azure OpenAI](./05-ImageGenerationOpenAI.md)
- [Running models locally with AI Toolkit, Docker, and Foundry Local](./06-LocalModelRunners.md)
- [Image and Video Generation with New Azure OpenAI Models](./07-ImageVideoGenerationNewModels.md)


👉 [Go to part 1 - completions, chat and functions](./01-lm-completions-functions.md)

---

## Sample Code Organization

All code samples for this lesson are located in the **[`samples/CoreSamples/`](../samples/CoreSamples/)** directory. This includes:

- **Chat & Completions**: `BasicChat-01MEAI`, `BasicChat-03Ollama`, `BasicChat-05AIFoundryModels`, etc.
- **Function Calling**: `MEAIFunctions`, `MEAIFunctionsAzureOpenAI`, `MEAIFunctionsOllama`
- **RAG Examples**: `RAGSimple-02MEAIVectorsMemory`, `RAGSimple-03MEAIVectorsAISearch`, etc.
- **Vision & Audio**: `Vision-01MEAI-GitHubModels`, `Audio-01-SpeechMic`, `Audio-02-RealTimeAudio`
- **Agents**: `AgentLabs-01-Simple`, `AgentLabs-02-Functions`, `AgentLabs-03-OpenAPIs`
- **Image Generation**: `ImageGeneration-01`
- **Video Generation**: `VideoGeneration-AzureSora-01`, `VideoGeneration-AzureSoraSDK-02`
- **Local Model Runners**: `DockerModels-02-MEAI-Chat`, `AIFoundryLocal-01-MEAI-Chat`
- **Deprecated (SK)**: See [`samples/deprecated/`](../samples/deprecated/) for archived Semantic Kernel samples

Each lesson document links directly to the relevant samples in this centralized location.

## Want to know more?

See the [Project Documentation](./docs/projects.md) for detailed breakdowns, dependencies, and demo instructions for every project in this lesson.

## Troubleshooting

Having issues with chat APIs? Check out our [Common Chat API Issues and Solutions](./docs/troubleshooting-chat-api.md) guide for help with:
- Understanding chat message lists and conversation history
- Fixing "response.Messages does not exist" errors
- Proper usage of GetResponseAsync with conversation context
