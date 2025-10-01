# 🧪 Generative AI .NET Samples Collection

This folder contains **60+ complete, runnable code samples** that demonstrate practical implementations of Generative AI in .NET applications. Each sample is designed to be self-contained, well-documented, and ready to run.

## 📁 Sample Categories

### 🔥 [CoreGenerativeAITechniques/](./CoreGenerativeAITechniques/)

**Multiple samples** covering the core concepts of Generative AI development in .NET, the concepts are explained in [Chapter 03](../03-CoreGenerativeAITechniques/readme.md):

#### 💬 **Chat Applications**

- **BasicChat-01MEAI** - Simple chat using Microsoft.Extensions.AI with GitHub Models
- **BasicChat-02SK** - Chat implementation using Semantic Kernel
- **BasicChat-03Ollama** - Local chat using Ollama models
- **BasicChat-04OllamaSK** - Ollama + Semantic Kernel integration
- **BasicChat-05AIFoundryModels** - Azure AI Foundry model integration
- **BasicChat-06OpenAIAPIs** - Direct OpenAI API usage
- **BasicChat-07Ollama-gpt-oss** - OpenAI GPT-OSS model with Ollama

#### 🔍 **RAG (Retrieval Augmented Generation)**

- **RAGSimple-01SK** - Basic RAG with Semantic Kernel
- **RAGSimple-02MEAIVectorsMemory** - In-memory vector store RAG
- **RAGSimple-03MEAIVectorsAISearch** - Azure AI Search integration
- **RAGSimple-04MEAIVectorsQdrant** - Qdrant vector database
- **RAGSimple-10SKOllama** - RAG with local Ollama models
- **RAGSimple-15Ollama-DeepSeekR1** - Advanced reasoning with DeepSeek-R1

#### 🎯 **Function Calling & Tools**

- **MEAIFunctions** - Function calling with Microsoft.Extensions.AI
- **MEAIFunctionsAzureOpenAI** - Azure OpenAI function integration
- **MEAIFunctionsOllama** - Local function calling with Ollama
- **SKFunctions01** - Semantic Kernel function examples

#### 🤖 **AI Agents**

- **AgentLabs-01-Simple** - Basic agent implementation
- **AgentLabs-02-Functions** - Agents with custom functions
- **AgentLabs-03-OpenAPIs** - OpenAPI specification integration
- **AgentLabs-03-PythonParksInformationServer** - Python service integration

#### 👁️ **Vision & Multimodal**

- **Vision-01MEAI-GitHubModels** - Image analysis with GitHub Models
- **Vision-02MEAI-Ollama** - Local vision models with Ollama
- **Vision-03MEAI-AOAI** - Azure OpenAI vision capabilities
- **Vision-04MEAI-AOAI-Spectre** - Advanced UI with Spectre Console

#### 🎵 **Audio Processing**

- **Audio-01-SpeechMic** - Speech recognition and synthesis
- **Audio-02-RealTimeAudio** - Real-time audio conversation

#### 🎨 **Image & Video Generation**

- **ImageGeneration-01** - DALL-E image generation
- **VideoGeneration-AzureSora-01** - Video generation with Azure Sora
- **VideoGeneration-AzureSoraSDK-02** - Sora SDK implementation

#### 🐳 **Local Model Runners**

- **DockerModels-01-SK-Chat** - Docker-based model hosting
- **DockerModels-02-MEAI-Chat** - MEAI with Docker models
- **AIToolkit-01-SK-Chat** - AI Toolkit integration
- **AIToolkit-02-MEAI-Chat** - AI Toolkit with MEAI
- **AIFoundryLocal-01-SK-Chat** - Local AI Foundry models
- **AIFoundryLocal-01-MEAI-Chat** - MEAI with local foundry

#### 🔗 **MCP (Model Context Protocol)**

- **MCP-01-HuggingFace** - Hugging Face MCP server integration
- **MCP-02-HuggingFace-Ollama** - MCP with local Ollama models

### 🏗️ [PracticalSamples/](./PracticalSamples/)

**Real-world applications** demonstrating enterprise patterns, the concepts are explained in [Chapter 04](../04-PracticalSamples/readme.md):

- **Aspire.MCP.Sample** - Complete .NET Aspire application with MCP integration
  - **McpSample.AppHost** - Aspire orchestration host
  - **McpSample.AspNetCoreServer** - ASP.NET Core web API
  - **McpSample.Chat** - Chat interface implementation
  - **McpSample.ServiceDefaults** - Shared service configurations

## 🚀 How to Use These Samples

### Prerequisites

- .NET 9 SDK or later
- Visual Studio 2022 or VS Code
- Git for cloning

### Quick Start

1. **Clone the repository:**

   ```bash
   git clone https://github.com/microsoft/Generative-AI-for-beginners-dotnet-workshop.git
   cd Generative-AI-for-beginners-dotnet-workshop/samples
   ```

2. **Choose your AI provider:**
   - **GitHub Models** (Free tier available) - Set up GitHub token
   - **Azure OpenAI** - Configure Azure credentials
   - **Ollama** (Local) - Install Ollama and pull models

3. **Run any sample:**

   ```bash
   cd CoreGenerativeAITechniques/BasicChat-01MEAI
   dotnet run
   ```

### 🔧 Configuration

Most samples support multiple AI providers. Common configuration patterns:

#### GitHub Models (Recommended for beginners)

```bash
# Set your GitHub token
export GITHUB_TOKEN="your_github_token_here"
```

#### Azure OpenAI

```bash
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "your_endpoint"
dotnet user-secrets set "AZURE_OPENAI_APIKEY" "your_api_key"
dotnet user-secrets set "AZURE_OPENAI_MODEL" "gpt-4o-mini"
```

#### Ollama (Local models)

```bash
# Install Ollama and pull a model
ollama pull phi4-mini
# Most samples will auto-detect local Ollama installation
```

## 🔧 Technologies Demonstrated

- **Microsoft.Extensions.AI** - Unified AI abstraction layer
- **Semantic Kernel** - AI orchestration framework
- **Azure OpenAI Services** - Enterprise AI services
- **GitHub Models** - Free tier AI models
- **Ollama** - Local model hosting
- **AI Foundry** - Model management platform
- **.NET Aspire** - Cloud-native orchestration
- **Model Context Protocol (MCP)** - AI service integration
- **Vector Databases** - Qdrant, Azure AI Search, In-Memory
- **Multimodal AI** - Vision, audio, and text processing

## 🎓 Learning Resources

Each sample includes:

- ✅ **Complete source code** with detailed comments
- ✅ **Project file** with all dependencies
- ✅ **Configuration examples** for multiple providers

## 🤝 Contributing

Found a bug or want to add a new sample?

1. **Report issues** - [Create an issue](https://github.com/microsoft/Generative-AI-for-beginners-dotnet-workshop/issues)
2. **Submit samples** - Fork, add your sample, and submit a PR
3. **Improve docs** - Help make the samples more accessible

## 📄 License

All samples are licensed under the MIT License. Feel free to use them in your own projects!

---

**🚀 Ready to start building?** Pick a sample that matches your skill level and dive in! Each sample is designed to teach specific concepts while being practical enough to use as a foundation for your own projects.
