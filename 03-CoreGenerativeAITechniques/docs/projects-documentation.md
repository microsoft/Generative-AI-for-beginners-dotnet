# Core Generative AI Techniques - Project Documentation

This document provides comprehensive documentation for the first half of the projects in the Core Generative AI Techniques lesson. Each project demonstrates different AI capabilities, frameworks, and integration patterns.

## Quick Navigation

- [01 Basic Chat Projects](#01-basic-chat-projects)
- [02 Functions Projects](#02-functions-projects)
- [03 RAG (Retrieval-Augmented Generation) Projects](#03-rag-retrieval-augmented-generation-projects)
- [04 Vision Analysis Projects](#04-vision-analysis-projects)
- [05 Audio Projects](#05-audio-projects)
- [06 Agents Projects](#06-agents-projects)

---

## 01 Basic Chat Projects

### BasicChat-01MEAI
**Short Description**: Basic chat implementation using Microsoft Extensions AI with GitHub Models API

**Dependencies**:
- `Microsoft.Extensions.AI` (9.6.0) - Core AI abstractions
- `Microsoft.Extensions.AI.AzureAIInference` (9.6.0-preview.1.25310.2) - Azure AI inference integration
- `Microsoft.Extensions.Configuration.UserSecrets` (9.0.6) - Configuration management

**Main Features**:
- Sentiment analysis of product reviews
- Uses GitHub Models API with Phi-3.5-MoE-instruct model
- Demonstrates basic prompt construction and response handling
- Environment variable and user secrets configuration

**How to Run/Demo**:
1. Set up GitHub token in user secrets: `dotnet user-secrets set "GITHUB_TOKEN" "your-token"`
2. Run: `dotnet run`
3. The app analyzes predefined product reviews and outputs sentiment analysis

**AI Techniques**: Text completion, sentiment analysis, prompt engineering
**Tags**: #MEAI #GitHubModels #Chat #SentimentAnalysis #BasicAI

### BasicChat-02SK
**Short Description**: Chat implementation using Semantic Kernel framework with OpenAI integration

**Dependencies**:
- `Microsoft.SemanticKernel` (1.55.0) - Semantic Kernel framework
- `Microsoft.Extensions.Configuration.UserSecrets` (9.0.2) - Configuration management

**Main Features**:
- Interactive chat using Semantic Kernel
- OpenAI client integration with GitHub Models
- Chat history management
- Uses Phi-3.5-mini-instruct model

**How to Run/Demo**:
1. Configure GitHub token in user secrets
2. Run: `dotnet run`
3. Interactive chat session with AI model

**AI Techniques**: Chat completion, conversation management, Semantic Kernel patterns
**Tags**: #SemanticKernel #OpenAI #GitHubModels #Chat #Interactive

### BasicChat-03Ollama
**Short Description**: Local AI chat using Ollama with Microsoft Extensions AI

**Dependencies**:
- `Microsoft.Extensions.AI` (9.6.0) - Core AI abstractions
- Ollama local server integration

**Main Features**:
- Local AI model execution via Ollama
- Sentiment analysis of product reviews
- No cloud dependencies required
- Uses phi4-mini model locally

**How to Run/Demo**:
1. Install and start Ollama: `ollama serve`
2. Pull model: `ollama pull phi4-mini`
3. Run: `dotnet run`
4. Analyzes reviews using local AI model

**AI Techniques**: Local AI inference, sentiment analysis, offline processing
**Tags**: #Ollama #LocalAI #MEAI #SentimentAnalysis #OfflineAI

### BasicChat-04OllamaSK
**Short Description**: Semantic Kernel integration with Ollama for local AI chat

**Dependencies**:
- `Microsoft.SemanticKernel` (1.54.0) - Semantic Kernel framework
- `Microsoft.SemanticKernel.Connectors.Ollama` (1.54.0-alpha) - Ollama connector
- `Microsoft.Extensions.Configuration.UserSecrets` (9.0.5) - Configuration

**Main Features**:
- Combines Semantic Kernel with local Ollama models
- Interactive chat with local AI
- No cloud API keys required
- Demonstrates local-first AI development

**How to Run/Demo**:
1. Start Ollama server
2. Pull required model (check Program.cs for model name)
3. Run: `dotnet run`
4. Interactive chat with local AI model

**AI Techniques**: Local AI inference, Semantic Kernel patterns, chat completion
**Tags**: #SemanticKernel #Ollama #LocalAI #Chat #Interactive

---

## 02 Functions Projects

### SKFunctions01
**Short Description**: Semantic Kernel with custom function plugins for weather data

**Dependencies**:
- `Microsoft.SemanticKernel` (1.55.0) - Core framework
- `Microsoft.SemanticKernel.Connectors.Ollama` (1.54.0-alpha) - Ollama support
- `Microsoft.Extensions.Configuration.UserSecrets` (9.0.5) - Configuration

**Main Features**:
- Custom weather plugin (`CityTemperaturePlugIn`)
- Function calling with AI models
- Interactive chat with tool access
- Supports both cloud and local models

**How to Run/Demo**:
1. Configure API credentials
2. Run: `dotnet run`
3. Ask weather-related questions: "What's the temperature in London?"
4. AI will use the custom function to provide weather data

**AI Techniques**: Function calling, plugin architecture, tool use
**Tags**: #SemanticKernel #Functions #Plugins #Weather #ToolUse

### MEAIFunctions
**Short Description**: Microsoft Extensions AI with function calling capabilities

**Dependencies**:
- `Microsoft.Extensions.AI` (9.6.0) - Core AI framework
- Function calling implementations

**Main Features**:
- Demonstrates MEAI function calling patterns
- Tool integration with AI models
- Custom function definitions
- Response handling with tool results

**How to Run/Demo**:
1. Set up API credentials
2. Run: `dotnet run`
3. Interact with AI using function-enabled prompts

**AI Techniques**: Function calling, tool integration, MEAI patterns
**Tags**: #MEAI #Functions #ToolCalling #FunctionIntegration

### MEAIFunctionsOllama
**Short Description**: Local function calling using Ollama with MEAI

**Dependencies**:
- `Microsoft.Extensions.AI` framework
- Ollama integration for local models
- Custom function implementations

**Main Features**:
- Local function calling without cloud dependencies
- Ollama model integration
- Tool use with local AI models
- Demonstrates offline AI capabilities

**How to Run/Demo**:
1. Start Ollama server
2. Ensure compatible model is available
3. Run: `dotnet run`
4. Test function calling with local AI

**AI Techniques**: Local function calling, offline AI, tool use
**Tags**: #MEAI #Ollama #LocalFunctions #ToolUse #OfflineAI

### MEAIFunctionsAzureOpenAI
**Short Description**: Azure OpenAI integration with MEAI function calling

**Dependencies**:
- `Microsoft.Extensions.AI` framework
- Azure OpenAI connectors
- Function calling capabilities

**Main Features**:
- Azure OpenAI service integration
- Production-ready function calling
- Enterprise AI scenarios
- Scalable AI function architecture

**How to Run/Demo**:
1. Configure Azure OpenAI credentials
2. Set up required environment variables
3. Run: `dotnet run`
4. Test function calling with Azure models

**AI Techniques**: Azure AI services, function calling, enterprise AI
**Tags**: #MEAI #AzureOpenAI #Functions #Enterprise #CloudAI

---

## 03 RAG (Retrieval-Augmented Generation) Projects

### RAGSimple-02MEAIVectorsMemory
**Short Description**: In-memory vector storage for RAG using MEAI

**Dependencies**:
- `Microsoft.Extensions.AI.Ollama` (9.5.0-preview.1.25265.7) - Ollama integration
- `Microsoft.Extensions.VectorData.Abstractions` (9.5.0) - Vector data abstractions
- `Microsoft.SemanticKernel.Connectors.InMemory` (1.54.0-preview) - In-memory vector store
- `MEAIVectorsShared` - Shared utilities

**Main Features**:
- In-memory vector database
- Document embedding and retrieval
- RAG pipeline implementation
- Local vector storage without external dependencies

**How to Run/Demo**:
1. Start Ollama with embedding model
2. Run: `dotnet run`
3. Add documents and query for relevant information
4. See RAG responses based on stored documents

**AI Techniques**: Vector embeddings, semantic search, RAG, in-memory storage
**Tags**: #RAG #VectorDB #Embeddings #MEAI #InMemory

### MEAIVectorsShared
**Short Description**: Shared utilities and models for vector operations

**Dependencies**:
- Common vector data models
- Utility functions for RAG projects

**Main Features**:
- Shared data models for vector operations
- Common utilities for embedding and retrieval
- Reusable components across RAG projects
- Abstraction layer for vector operations

**How to Run/Demo**:
This is a shared library referenced by other RAG projects.

**AI Techniques**: Vector data modeling, shared abstractions
**Tags**: #Shared #VectorData #Utilities #RAG

### RAGSimple-03MEAIVectorsAISearch
**Short Description**: Azure AI Search integration for production RAG scenarios

**Dependencies**:
- Azure AI Search connectors
- MEAI framework integration
- Vector search capabilities

**Main Features**:
- Azure AI Search integration
- Production-scale vector search
- Hybrid search capabilities (text + vector)
- Enterprise RAG implementation

**How to Run/Demo**:
1. Set up Azure AI Search service
2. Configure search credentials
3. Run: `dotnet run`
4. Index documents and perform RAG queries

**AI Techniques**: Azure AI Search, hybrid search, production RAG
**Tags**: #RAG #AzureAISearch #Production #HybridSearch #Enterprise

### RAGSimple-04MEAIVectorsQdrant
**Short Description**: Qdrant vector database integration for RAG

**Dependencies**:
- Qdrant vector database connectors
- MEAI integration
- Vector storage and retrieval

**Main Features**:
- Qdrant vector database integration
- High-performance vector search
- Persistent vector storage
- Scalable RAG implementation

**How to Run/Demo**:
1. Start Qdrant server (Docker recommended)
2. Configure Qdrant connection
3. Run: `dotnet run`
4. Store and retrieve documents using vector similarity

**AI Techniques**: Qdrant vector DB, persistent storage, scalable RAG
**Tags**: #RAG #Qdrant #VectorDB #Persistent #Scalable

### RAGSimple-10SKOllama
**Short Description**: Semantic Kernel RAG implementation with local Ollama models

**Dependencies**:
- `Microsoft.SemanticKernel` framework
- Ollama integration
- Vector memory connectors

**Main Features**:
- Local RAG with Semantic Kernel
- Ollama model integration
- Memory-based document storage
- Offline RAG capabilities

**How to Run/Demo**:
1. Start Ollama with text and embedding models
2. Run: `dotnet run`
3. Add documents to memory
4. Query for information with RAG responses

**AI Techniques**: Local RAG, Semantic Kernel memory, offline processing
**Tags**: #SemanticKernel #RAG #Ollama #LocalRAG #Memory

### RAGSimple-15Ollama-DeepSeekR1
**Short Description**: RAG implementation using DeepSeek R1 model via Ollama

**Dependencies**:
- Ollama integration
- DeepSeek R1 model support
- RAG pipeline components

**Main Features**:
- Advanced reasoning with DeepSeek R1
- Local RAG with reasoning capabilities
- Enhanced document understanding
- Advanced query processing

**How to Run/Demo**:
1. Pull DeepSeek R1 model: `ollama pull deepseek-r1`
2. Run: `dotnet run`
3. Test RAG with complex reasoning queries

**AI Techniques**: Advanced reasoning, local RAG, DeepSeek models
**Tags**: #RAG #DeepSeek #Reasoning #Ollama #AdvancedAI

---

## 04 Vision Analysis Projects

### Vision-01MEAI-GitHubModels
**Short Description**: Vision AI using GitHub Models API with Microsoft Extensions AI

**Dependencies**:
- `Microsoft.Extensions.AI` (9.5.0) - Core AI framework
- `Microsoft.Extensions.AI.AzureAIInference` (9.5.0-preview.1.25265.7) - Azure inference
- `OpenCvSharp4.runtime.win` (4.10.0.20241108) - Image processing
- `Microsoft.Extensions.Configuration.UserSecrets` (9.0.5) - Configuration

**Main Features**:
- Image analysis using GitHub Models
- Multiple image formats support
- OCR and image understanding
- Sample images included (receipt, license, shoes)

**How to Run/Demo**:
1. Configure GitHub token
2. Run: `dotnet run`
3. Analyzes included sample images
4. Extracts text and provides image descriptions

**AI Techniques**: Computer vision, OCR, image analysis, multimodal AI
**Tags**: #Vision #MEAI #GitHubModels #OCR #ImageAnalysis

### Vision-02MEAI-Ollama
**Short Description**: Local vision AI using Ollama with multimodal models

**Dependencies**:
- MEAI framework
- Ollama multimodal model support
- Local image processing

**Main Features**:
- Local vision model processing
- Multimodal AI capabilities
- No cloud dependencies
- Image understanding and description

**How to Run/Demo**:
1. Pull multimodal model: `ollama pull llava` or similar
2. Run: `dotnet run`
3. Process images locally
4. Get image descriptions and analysis

**AI Techniques**: Local vision AI, multimodal models, offline processing
**Tags**: #Vision #Ollama #LocalAI #Multimodal #OfflineVision

### Vision-03MEAI-AOAI
**Short Description**: Azure OpenAI vision capabilities with MEAI

**Dependencies**:
- MEAI framework
- Azure OpenAI connectors
- Vision model integration

**Main Features**:
- Azure OpenAI Vision integration
- GPT-4V or similar model usage
- Production vision AI
- Enterprise-grade image analysis

**How to Run/Demo**:
1. Configure Azure OpenAI credentials
2. Run: `dotnet run`
3. Upload or process images
4. Get detailed image analysis

**AI Techniques**: Azure OpenAI Vision, GPT-4V, enterprise vision AI
**Tags**: #Vision #AzureOpenAI #MEAI #GPT4V #Enterprise

### Vision-04MEAI-AOAI-Spectre
**Short Description**: Advanced vision processing with Spectre.Console UI

**Dependencies**:
- MEAI framework
- Azure OpenAI integration
- Spectre.Console for enhanced UI
- Advanced image processing

**Main Features**:
- Rich console UI with Spectre.Console
- Advanced image analysis workflows
- Interactive vision AI application
- Enhanced user experience

**How to Run/Demo**:
1. Set up Azure OpenAI credentials
2. Run: `dotnet run`
3. Use interactive menu for image processing
4. Enhanced visual feedback and results

**AI Techniques**: Vision AI, interactive UI, enhanced UX
**Tags**: #Vision #AzureOpenAI #SpectreConsole #Interactive #UX

---

## 05 Audio Projects

### Audio-01-SpeechMic
**Short Description**: Speech recognition and audio processing from microphone input

**Dependencies**:
- Audio processing libraries
- Speech recognition services
- Microphone access components

**Main Features**:
- Real-time microphone input
- Speech-to-text conversion
- Audio preprocessing
- Voice command recognition

**How to Run/Demo**:
1. Ensure microphone permissions
2. Run: `dotnet run`
3. Speak into microphone
4. See real-time speech transcription

**AI Techniques**: Speech recognition, audio processing, real-time AI
**Tags**: #Audio #SpeechToText #Microphone #RealTime #Voice

### Audio-02-RealTimeAudio
**Short Description**: Real-time audio processing and AI integration

**Dependencies**:
- Real-time audio libraries
- AI audio processing
- Streaming audio components

**Main Features**:
- Real-time audio stream processing
- Low-latency audio AI
- Continuous audio analysis
- Streaming audio capabilities

**How to Run/Demo**:
1. Configure audio devices
2. Run: `dotnet run`
3. Process audio in real-time
4. See continuous AI analysis

**AI Techniques**: Real-time audio AI, stream processing, low-latency inference
**Tags**: #Audio #RealTime #Streaming #LowLatency #AudioAI

---

## 06 Agents Projects

### AgentLabs-01-Simple
**Short Description**: Basic AI agent using Azure AI Projects SDK

**Dependencies**:
- `Azure.AI.Agents.Persistent` (1.1.0-beta.1) - Agent persistence
- `Azure.AI.Projects` (1.0.0-beta.9) - Azure AI Projects
- `Azure.Identity` (1.14.0) - Authentication
- `Microsoft.Extensions.Configuration.UserSecrets` (9.0.5) - Configuration

**Main Features**:
- Simple AI agent implementation
- Azure AI Projects integration
- Agent state management
- Basic agent workflows

**How to Run/Demo**:
1. Set up Azure AI Projects credentials
2. Run: `dotnet run`
3. Interact with the AI agent
4. See agent responses and state management

**AI Techniques**: AI agents, state management, agent orchestration
**Tags**: #Agents #AzureAI #AgentOrchestration #StateManagement

### AgentLabs-02-Functions
**Short Description**: AI agents with custom function capabilities

**Dependencies**:
- Azure AI Projects SDK
- Custom function implementations
- Agent function calling

**Main Features**:
- Agents with tool access
- Custom function definitions
- Agent-tool integration
- Complex agent workflows

**How to Run/Demo**:
1. Configure agent and function credentials
2. Run: `dotnet run`
3. Ask agent to perform tasks requiring tools
4. See agent function calling in action

**AI Techniques**: Agent function calling, tool integration, complex workflows
**Tags**: #Agents #Functions #ToolUse #AgentWorkflows

### AgentLabs-03-OpenAPIs
**Short Description**: Agents with OpenAPI integration for external service access

**Dependencies**:
- Azure AI Projects SDK
- OpenAPI client generation
- External API integration

**Main Features**:
- OpenAPI specification integration
- External service access
- Agent API orchestration
- Complex multi-service workflows

**How to Run/Demo**:
1. Set up API credentials and endpoints
2. Run: `dotnet run`
3. Request agent to use external APIs
4. See multi-service agent orchestration

**AI Techniques**: Agent API integration, OpenAPI, service orchestration
**Tags**: #Agents #OpenAPI #APIIntegration #ServiceOrchestration

---

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Visual Studio 2022 or VS Code
- Appropriate API keys/tokens for cloud services
- Ollama installed for local projects

### Configuration
Most projects use user secrets for configuration. Set up credentials using:
```bash
dotnet user-secrets set "GITHUB_TOKEN" "your-token"
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "your-endpoint"
dotnet user-secrets set "AZURE_OPENAI_APIKEY" "your-key"
```

### Running Projects
1. Navigate to project directory
2. Install dependencies: `dotnet restore`
3. Run project: `dotnet run`

### Common Issues
- **API Keys**: Ensure all required API keys are configured
- **Ollama**: For local projects, verify Ollama is running and models are pulled
- **Dependencies**: Some projects require specific versions - check .csproj files

---

## Additional Resources
- [Semantic Kernel Documentation](https://learn.microsoft.com/semantic-kernel/)
- [Microsoft Extensions AI](https://learn.microsoft.com/dotnet/ai/)
- [Azure OpenAI Service](https://azure.microsoft.com/services/openai-service/)
- [Ollama Documentation](https://ollama.com/docs)