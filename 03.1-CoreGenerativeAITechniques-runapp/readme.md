# Core Generative AI Techniques - Single-File Demos

> 💡 *This lesson uses .NET 10's `dotnet run <file>.cs` feature for ultra-simple AI demos. Each file is self-contained and runnable! Don't have .NET 10 yet? Use the provided fallback scripts (`run-demo.sh` or `run-demo.ps1`) that work with .NET 9.*

Welcome to the modernized version of Core Generative AI Techniques! This lesson demonstrates the same powerful AI concepts as the full [Lesson 03](../03-CoreGenerativeAITechniques/readme.md), but with simplified **single-file C# demos** that showcase the elegance of .NET 10's direct file execution.

---

## What you'll learn

- 🌟 **LLM Completions** - Text generation and analysis
- 💬 **Interactive Chat** - Conversational AI with memory
- 🔧 **Function Calling** - Extending AI with custom tools  
- 🔍 **RAG (Retrieval-Augmented Generation)** - AI + knowledge bases
- 📊 **Structured Output** - Getting consistent JSON responses
- 👁️ **Multimodal AI** - Vision and image analysis
- 🖼️ **Image Generation** - Creating images with AI
- 🤖 **Local Models** - Running AI locally with Ollama
- ☁️ **Cloud Providers** - Azure OpenAI integration
- 🧠 **Vector Search** - Advanced semantic search

## Prerequisites

- **.NET 10 SDK** - Required for `dotnet run file.cs` functionality
- **GitHub Token** with access to GitHub Models (for most demos)
- **Azure OpenAI** (optional, for Azure-specific demos)
- **Ollama** (optional, for local AI demos)

## Quick Setup

1. **Set your GitHub Token** (for GitHub Models demos):
   ```bash
   # Environment variable (recommended)
   export GITHUB_TOKEN="your_github_token_here"
   
   # OR user secrets for development
   dotnet user-secrets set "GITHUB_TOKEN" "your_github_token_here"
   ```

2. **Navigate to the demo directory**:
   ```bash
   cd 03.1-CoreGenerativeAITechniques-runapp/src
   ```

3. **Run any demo**:
   ```bash
   dotnet run 01-llm-completion.cs
   ```

## Single-File Demos

### Core AI Techniques

#### 1. LLM Text Completion 📝
**File**: `01-llm-completion.cs`  
**Concept**: Basic text completions with GitHub Models

```bash
dotnet run 01-llm-completion.cs
```

Learn:
- Setting up GitHub Models client
- Creating effective prompts for sentiment analysis
- Processing single-turn AI responses
- Handling authentication and errors

#### 2. Interactive Chat Flow 💬
**File**: `02-chat-flow.cs`  
**Concept**: Stateful conversations with memory

```bash
dotnet run 02-chat-flow.cs
```

Discover:
- Maintaining conversation history
- Interactive chat experiences with streaming
- Building context-aware conversations
- User input handling and response formatting

#### 3. Functions and Plugins 🔧
**File**: `03-functions-and-plugins.cs`  
**Concept**: Extending AI with custom tools

```bash
dotnet run 03-functions-and-plugins.cs
```

Master:
- Function calling with Microsoft.Extensions.AI
- Creating custom AI tools/plugins
- Automatic function invocation
- Tool descriptions and parameters

#### 4. Retrieval-Augmented Generation (RAG) 🔍
**File**: `04-retrieval-augmented-generation.cs`  
**Concept**: Simple RAG with keyword search

```bash
dotnet run 04-retrieval-augmented-generation.cs
```

Explore:
- In-memory knowledge stores
- Context injection for AI responses
- Building knowledge-aware applications
- Simple search and retrieval patterns

#### 5. Structured Output 📊
**File**: `05-structured-output.cs`  
**Concept**: Getting consistent JSON responses

```bash
dotnet run 05-structured-output.cs
```

Learn:
- Structured data extraction
- JSON schema compliance
- Type-safe AI responses
- Business data processing patterns

#### 6. Multimodal AI (Vision) 👁️
**File**: `06-multimodal.cs`  
**Concept**: Analyzing images with AI

```bash
dotnet run 06-multimodal.cs
```

Discover:
- Vision-enabled AI models
- Image analysis and description
- URL and local file processing
- Multimodal conversation flows

### Provider-Specific Demos

#### 7. Local AI with Ollama 🏠
**File**: `07-ollama-local-models.cs`  
**Concept**: Running AI models locally

```bash
dotnet run 07-ollama-local-models.cs
```

Features:
- No API keys required
- Privacy-focused local processing
- Phi4-mini model integration
- Connection troubleshooting

#### 8. Azure Image Generation 🎨
**File**: `08-azure-image-generation.cs`  
**Concept**: Creating images with DALL-E

```bash
dotnet run 08-azure-image-generation.cs
```

Learn:
- Azure OpenAI DALL-E integration
- Image generation parameters
- Saving and displaying generated images
- Cross-platform file handling

#### 9. Semantic Kernel Chat 🧠
**File**: `09-semantic-kernel-chat.cs`  
**Concept**: Interactive chat with SK framework

```bash
dotnet run 09-semantic-kernel-chat.cs
```

Explore:
- Semantic Kernel framework
- Streaming conversation experiences
- Advanced chat capabilities
- Framework comparison with MEAI

#### 10. Azure Functions 🔧
**File**: `10-azure-functions.cs`  
**Concept**: Function calling with Azure OpenAI

```bash
dotnet run 10-azure-functions.cs
```

Master:
- Azure OpenAI function calling
- Multiple function definitions
- Weather, time, and calculation tools
- Enterprise-grade AI integration

#### 11. Advanced RAG with Vectors 🔍
**File**: `11-advanced-rag-vectors.cs`  
**Concept**: Vector-based semantic search

```bash
dotnet run 11-advanced-rag-vectors.cs
```

Advanced topics:
- Vector embeddings and similarity
- Semantic search capabilities  
- Local embedding generation
- Movie recommendation system

## Key Features of These Demos

### ✨ Simplified Architecture
- **Direct file execution** with .NET 10's `dotnet run file.cs`
- **Top-level statements** - Minimal boilerplate code
- **Self-contained** - Each demo focuses on one core concept

### 🚀 Modern .NET 10 Features
- **Single-file execution** - No project files needed
- **Implicit global usings** - Cleaner, more focused code
- **Latest AI libraries** - Microsoft.Extensions.AI integration

### 🎯 Educational Progressive Learning
- **11 comprehensive demos** covering all major AI techniques
- **Multiple provider examples** - GitHub Models, Azure OpenAI, Ollama
- **Real-world scenarios** - Practical applications you can build upon
- **Best practices** - Error handling, authentication, logging

### 🔧 Provider Flexibility
- **GitHub Models** - Free tier for learning and experimentation
- **Azure OpenAI** - Enterprise-grade cloud AI services
- **Ollama** - Privacy-focused local AI processing
- **Easy switching** - Similar patterns across providers

## Setup Instructions by Provider

### GitHub Models (Recommended for Learning)
```bash
# Set your GitHub token
export GITHUB_TOKEN="your_github_token_here"

# Works with demos: 01, 02, 03, 04, 05, 06, 09
dotnet run 01-llm-completion.cs
```

### Azure OpenAI (Enterprise/Production)
```bash
# Set Azure configuration
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "AZURE_OPENAI_APIKEY" "your-api-key"
dotnet user-secrets set "AZURE_OPENAI_MODEL" "gpt-4o-mini"

# Works with demos: 08, 10
dotnet run 08-azure-image-generation.cs
```

### Ollama (Local/Privacy)
```bash
# Start Ollama and pull required models
ollama serve
ollama pull phi4-mini
ollama pull all-minilm

# Works with demos: 07, 11
dotnet run 07-ollama-local-models.cs
```

## Running the Demos

### Option 1: .NET 10 Direct Execution (Recommended)

With .NET 10 installed, running demos is incredibly simple:

```bash
# Navigate to the demos folder
cd 03.1-CoreGenerativeAITechniques-runapp/src

# Run any demo directly
dotnet run 01-llm-completion.cs
dotnet run 02-chat-flow.cs
dotnet run 03-functions-and-plugins.cs
dotnet run 04-retrieval-augmented-generation.cs
dotnet run 05-structured-output.cs
dotnet run 06-multimodal.cs
dotnet run 07-ollama-local-models.cs
dotnet run 08-azure-image-generation.cs
dotnet run 09-semantic-kernel-chat.cs
dotnet run 10-azure-functions.cs
dotnet run 11-advanced-rag-vectors.cs
```

### Option 2: Fallback Scripts (If .NET 10 Not Available)

If you don't have .NET 10 yet, use the provided scripts that simulate the single-file execution:

**Linux/macOS (Bash):**
```bash
# Navigate to the demos folder
cd 03.1-CoreGenerativeAITechniques-runapp/src

# Make script executable (first time only)
chmod +x run-demo.sh

# Run any demo
./run-demo.sh 01-llm-completion.cs
./run-demo.sh 02-chat-flow.cs
./run-demo.sh 06-multimodal.cs
```

**Windows (PowerShell):**
```powershell
# Navigate to the demos folder
cd 03.1-CoreGenerativeAITechniques-runapp/src

# Run any demo
.\run-demo.ps1 01-llm-completion.cs
.\run-demo.ps1 02-chat-flow.cs
.\run-demo.ps1 06-multimodal.cs
```

> 💡 **How the scripts work**: They create a temporary .NET project, copy your demo file as `Program.cs`, include all necessary NuGet packages, run the demo, and clean up automatically.

Each demo is completely self-contained and demonstrates different aspects of Generative AI development with .NET.

## Learning Paths

Choose your learning journey based on your goals:

### 🚀 Quick Start Path (1-2 hours)
Perfect for getting started quickly:
1. **01-llm-completion.cs** - Basic AI text generation  
2. **02-chat-flow.cs** - Interactive conversations
3. **03-functions-and-plugins.cs** - AI with custom tools

### 🎓 Comprehensive Learning Path (3-4 hours)  
Complete understanding of all techniques:
1. Start with Quick Start demos (1-3)
2. **04-retrieval-augmented-generation.cs** - Knowledge integration
3. **05-structured-output.cs** - Reliable data extraction
4. **06-multimodal.cs** - Vision and image analysis
5. **09-semantic-kernel-chat.cs** - Advanced framework features

### 🖼️ Multimodal AI Focus Path (2-3 hours)
For image and visual AI applications:
1. **06-multimodal.cs** - Vision analysis fundamentals
2. **08-azure-image-generation.cs** - Creating images with AI
3. **05-structured-output.cs** - Extracting data from visual content

### 🏠 Privacy & Local AI Path (2 hours)
For local, privacy-focused development:
1. **07-ollama-local-models.cs** - Local text generation
2. **11-advanced-rag-vectors.cs** - Local vector search
3. **04-retrieval-augmented-generation.cs** - Simple RAG patterns

### ☁️ Enterprise & Cloud Path (2-3 hours)
For production Azure deployments:  
1. **08-azure-image-generation.cs** - Azure DALL-E integration
2. **10-azure-functions.cs** - Azure OpenAI function calling
3. **09-semantic-kernel-chat.cs** - Advanced framework patterns

## Common Issues & Solutions

### .NET 10 Not Available?
- **.NET 10 is preferred** for the modern `dotnet run file.cs` syntax
- **Fallback option available**: Use the provided scripts in the `src/` folder:
  - **Linux/macOS**: `./run-demo.sh <demo-file.cs>`
  - **Windows**: `.\run-demo.ps1 <demo-file.cs>`
- **Alternative**: Use the project-based examples in [Lesson 03](../03-CoreGenerativeAITechniques/readme.md)
- Download the latest .NET 10 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com)

### Authentication Errors
- **GitHub Models**: Ensure your `GITHUB_TOKEN` has GitHub Models access
- **Azure OpenAI**: Verify endpoint URL and API key in user secrets
- **Ollama**: Check that `ollama serve` is running and models are pulled

### Missing Dependencies
Each demo is self-contained but relies on these NuGet packages being available:
- `Microsoft.Extensions.AI` (9.7.1+)
- `Microsoft.Extensions.AI.AzureAIInference` (9.6.0-preview.1+)
- `Microsoft.SemanticKernel.Connectors.InMemory` (1.62.0-preview+)

### Model Availability
- **GitHub Models**: Free tier has rate limits, consider upgrading for production
- **Azure OpenAI**: Ensure your subscription has the required model deployments
- **Ollama**: Pull required models: `ollama pull phi4-mini` and `ollama pull all-minilm`

## Next Steps

### 🛠️ Build Your Own
- **Experiment** with different prompts and parameters
- **Combine** techniques from multiple demos  
- **Create** your own single-file AI applications
- **Adapt** examples for your specific use cases

### 📚 Continue Learning
- **[Lesson 04 - Practical Samples](../04-PracticalSamples/readme.md)** - Complete AI applications
- **[Lesson 05 - SpaceAINet](../05-AppCreatedWithGenAI/SpaceAINet/README.md)** - Full-scale AI-powered game
- **Original [Lesson 03](../03-CoreGenerativeAITechniques/readme.md)** - Detailed project-based examples

### 🤝 Contribute
- **Share** your own single-file AI demos
- **Report** issues or suggest improvements
- **Extend** examples with new AI capabilities

---

> 🎉 **Congratulations!** You've learned the core techniques of Generative AI with .NET. These patterns form the foundation for building sophisticated AI-powered applications.

**Happy coding with .NET 10 and AI! 🚀**