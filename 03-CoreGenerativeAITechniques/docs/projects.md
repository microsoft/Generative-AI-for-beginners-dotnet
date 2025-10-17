# Core Generative AI Techniques – Project Documentation

Welcome to the documentation for all hands-on projects in the `03-CoreGenerativeAITechniques` lesson. This page provides an overview, dependencies, features, and demo instructions for each project. Use the index below to jump to any project.

---

## Index

- [BasicChat-01MEAI](#basicchat-01meai)
- [BasicChat-02SK](#basicchat-02sk)
- [BasicChat-03Ollama](#basicchat-03ollama)
- [BasicChat-04OllamaSK](#basicchat-04ollamask)
- [BasicChat-10ConversationHistory](#basicchat-10conversationhistory)
- [SKFunctions01](#skfunctions01)
- [RAGSimple-01SK](#ragsimple-01sk)
- [RAGSimple-02MEAIVectorsMemory](#ragsimple-02meaivectorsmemory)
- [RAGSimple-03MEAIVectorsAISearch](#ragsimple-03meaivectorsaisearch)
- [RAGSimple-04MEAIVectorsQdrant](#ragsimple-04meaivectorsqdrant)
- [RAGSimple-10SKOllama](#ragsimple-10skollama)
- [Vision-01MEAI-GitHubModels](#vision-01meai-githubmodels)
- [Vision-02MEAI-Ollama](#vision-02meai-ollama)
- [Vision-03MEAI-AOAI](#vision-03meai-aoai)
- [Vision-04MEAI-AOAI-Spectre](#vision-04meai-aoai-spectre)
- [Audio-01-SpeechMic](#audio-01-speechmic)
- [Audio-02-RealTimeAudio](#audio-02-realtimeaudio)
- [ImageGeneration-01](#imagegeneration-01)
- [VideoGeneration-AzureSora-01](#videogeneration-azuresora-01)
- [VideoGeneration-AzureSoraSDK-02](#videogeneration-azuresorasdk-02)
- [AgentLabs-01-Simple](#agentlabs-01-simple)
- [AgentLabs-02-Functions](#agentlabs-02-functions)
- [AgentLabs-03-OpenAPIs](#agentlabs-03-openapis)
- [AIToolkit-01-SK-Chat](#aitoolkit-01-sk-chat)
- [AIToolkit-02-MEAI-Chat](#aitoolkit-02-meai-chat)
- [DockerModels-01-SK-Chat](#dockermodels-01-sk-chat)
- [DockerModels-02-MEAI-Chat](#dockermodels-02-meai-chat)
- [OpenAI-FileProcessing-Pdf-01](#openai-fileprocessing-pdf-01)
- [MEAIFunctions](#meaifunctions)
- [MEAIFunctionsAzureOpenAI](#meaifunctionsazureopenai)
- [MEAIFunctionsOllama](#meaifunctionsollama)
- [MEAIVectorsShared](#meaivectorsshared)

---

<!-- Project documentation sections will follow here. -->

## BasicChat-01MEAI

**Description:**
A minimal .NET 9 console chat app using Microsoft.Extensions.AI and Azure OpenAI for LLM completions and chat.

**Dependencies:**
- Microsoft.Extensions.AI: Core AI abstractions
- Microsoft.Extensions.AI.AzureAIInference: Azure OpenAI integration
- Microsoft.Extensions.Configuration.*: App configuration and secrets

**Main Features:**
- Simple chat with Azure OpenAI
- User secrets for API keys
- Easily swappable AI provider

**How to Run:**
1. Set Azure OpenAI secrets (see setup guide)
2. `dotnet run` in `BasicChat-01MEAI`

**Tags:** #AzureOpenAI #Chat #MEAI

---

## BasicChat-02SK

**Description:**
A .NET 9 console chat app using Semantic Kernel for LLM orchestration and function calling.

**Dependencies:**
- Microsoft.SemanticKernel: LLM orchestration
- Microsoft.Extensions.Configuration.*: App configuration and secrets

**Main Features:**
- Chat with function calling
- Semantic Kernel plugins

**How to Run:**
1. Set secrets as needed
2. `dotnet run` in `BasicChat-02SK`

**Tags:** #SemanticKernel #Chat #Functions

---

## BasicChat-03Ollama

**Description:**
A .NET 9 console chat app using Ollama (local LLMs) via Microsoft.Extensions.AI.Ollama.

**Dependencies:**
- Microsoft.Extensions.AI.Ollama: Local LLM integration
- Microsoft.Extensions.Configuration.UserSecrets: Secrets

**Main Features:**
- Local LLM chat (Ollama)
- No cloud required

**How to Run:**
1. Start Ollama and pull a model (see setup guide)
2. `dotnet run` in `BasicChat-03Ollama`

**Tags:** #Ollama #Chat #LocalModels

---

## BasicChat-04OllamaSK

**Description:**
A .NET 9 console chat app using Semantic Kernel and Ollama connector for advanced orchestration.

**Dependencies:**
- Microsoft.SemanticKernel: LLM orchestration
- Microsoft.SemanticKernel.Connectors.Ollama: Ollama connector
- Microsoft.Extensions.Configuration: App config

**Main Features:**
- Semantic Kernel + Ollama
- Function calling

**How to Run:**
1. Start Ollama
2. `dotnet run` in `BasicChat-04OllamaSK`

**Tags:** #SemanticKernel #Ollama #Chat #Functions

---

## BasicChat-10ConversationHistory

**Description:**
A .NET 9 console app demonstrating the **correct way** to handle conversation history with `GetResponseAsync`. This example addresses the common issue where developers try to use `response.Messages` (which doesn't exist).

**Dependencies:**
- Microsoft.Extensions.AI.Ollama: Local LLM integration
- Microsoft.Extensions.Configuration.UserSecrets: Configuration

**Main Features:**
- Proper conversation history management
- Shows correct usage of `response.Text` or `response.Message`
- Demonstrates three different ways to add AI responses to conversation
- Comprehensive documentation on common mistakes

**How to Run:**
1. Start Ollama: `ollama serve`
2. Pull model: `ollama pull phi4-mini`
3. `dotnet run` in `BasicChat-10ConversationHistory`
4. Type "conversation" to start chatting

**Common Issue Solved:**
This example specifically addresses the error: `'ChatCompletion' does not contain a definition for 'Messages'`

See the [README](../src/BasicChat-10ConversationHistory/README.md) for detailed explanation of the issue and solutions.

**Tags:** #Ollama #MEAI #Chat #ConversationHistory #Troubleshooting

---

## SKFunctions01

**Description:**
Demonstrates Semantic Kernel function calling in .NET 9.

**Dependencies:**
- Microsoft.SemanticKernel: LLM orchestration
- Microsoft.SemanticKernel.Connectors.Ollama: Ollama connector
- Microsoft.Extensions.Configuration.UserSecrets: Secrets

**Main Features:**
- Function calling with LLMs
- Pluggable connectors

**How to Run:**
1. Set up secrets
2. `dotnet run` in `SKFunctions01`

**Tags:** #SemanticKernel #Functions #Ollama

---

## RAGSimple-01SK

**Description:**
A simple Retrieval-Augmented Generation (RAG) demo using Semantic Kernel and local embeddings.

**Dependencies:**
- Microsoft.KernelMemory.SemanticKernelPlugin: Memory plugin
- Microsoft.SemanticKernel: LLM orchestration
- SmartComponents.LocalEmbeddings.SemanticKernel: Local vector search

**Main Features:**
- RAG with local embeddings
- Semantic Kernel memory

**How to Run:**
1. `dotnet run` in `RAGSimple-01SK`

**Tags:** #RAG #SemanticKernel #Embeddings

---

## RAGSimple-02MEAIVectorsMemory

**Description:**
RAG demo using Microsoft.Extensions.AI.Ollama and in-memory vector store.

**Dependencies:**
- Microsoft.Extensions.AI.Ollama: Local LLM
- Microsoft.Extensions.VectorData.Abstractions: Vector store
- Microsoft.SemanticKernel.Connectors.InMemory: In-memory vector search

**Main Features:**
- RAG with Ollama and in-memory vectors

**How to Run:**
1. Start Ollama
2. `dotnet run` in `RAGSimple-02MEAIVectorsMemory`

**Tags:** #RAG #Ollama #Vectors #LocalModels

---

## RAGSimple-03MEAIVectorsAISearch

**Description:**
RAG demo using Azure AI Search and Ollama for hybrid search.

**Dependencies:**
- Azure.Identity: Azure auth
- Microsoft.Extensions.VectorData.Abstractions: Vector store
- Microsoft.SemanticKernel.Connectors.AzureAISearch: Azure AI Search
- Microsoft.Extensions.AI.Ollama: Local LLM

**Main Features:**
- Hybrid RAG: Azure AI Search + Ollama

**How to Run:**
1. Set Azure secrets
2. Start Ollama
3. `dotnet run` in `RAGSimple-03MEAIVectorsAISearch`

**Tags:** #RAG #AzureAI #Ollama #HybridSearch

---

## RAGSimple-04MEAIVectorsQdrant

**Description:**
RAG demo using Qdrant vector database and Ollama.

**Dependencies:**
- Microsoft.Extensions.AI.Ollama: Local LLM
- Microsoft.Extensions.VectorData.Abstractions: Vector store
- Microsoft.SemanticKernel.Connectors.Qdrant: Qdrant connector

**Main Features:**
- RAG with Qdrant
- Local LLM

**How to Run:**
1. Start Qdrant and Ollama
2. `dotnet run` in `RAGSimple-04MEAIVectorsQdrant`

**Tags:** #RAG #Qdrant #Ollama #Vectors

---

## RAGSimple-10SKOllama

**Description:**
Advanced RAG demo using KernelMemory, Ollama, and Spectre.Console for CLI output.

**Dependencies:**
- Microsoft.KernelMemory.*: Memory and AI
- Microsoft.SemanticKernel: LLM orchestration
- Spectre.Console: Rich CLI output

**Main Features:**
- RAG with KernelMemory
- Ollama integration
- CLI output

**How to Run:**
1. Start Ollama
2. `dotnet run` in `RAGSimple-10SKOllama`

**Tags:** #RAG #KernelMemory #Ollama #CLI

---

## Vision-01MEAI-GitHubModels

**Description:**
Vision demo using Azure OpenAI and GitHub models for image analysis.

**Dependencies:**
- Microsoft.Extensions.AI: Core AI
- Microsoft.Extensions.AI.AzureAIInference: Azure OpenAI
- OpenCvSharp4.runtime.win: Image processing

**Main Features:**
- Vision analysis with LLMs
- Sample images included

**How to Run:**
1. Set Azure secrets
2. `dotnet run` in `Vision-01MEAI-GitHubModels`

**Tags:** #Vision #AzureOpenAI #GitHubModels #ImageAnalysis

**Screenshots:**
- See `images/german-receipt.jpg`, `images/license.jpg`, `images/running-shoes.jpg`

---

## Vision-02MEAI-Ollama

**Description:**
Vision demo using Ollama for local image analysis.

**Dependencies:**
- Microsoft.Extensions.AI: Core AI
- Microsoft.Extensions.AI.Ollama: Local LLM
- OpenCvSharp4.runtime.win: Image processing

**Main Features:**
- Local vision analysis
- Sample images included

**How to Run:**
1. Start Ollama
2. `dotnet run` in `Vision-02MEAI-Ollama`

**Tags:** #Vision #Ollama #ImageAnalysis #LocalModels

**Screenshots:**
- See `images/german-receipt.jpg`, `images/license.jpg`, `images/running-shoes.jpg`

---

## Vision-03MEAI-AOAI

**Description:**
Vision demo using Azure OpenAI and OpenCV for advanced image analysis.

**Dependencies:**
- Azure.AI.OpenAI: Azure OpenAI
- OpenCvSharp4: Image processing

**Main Features:**
- Vision with Azure OpenAI
- Video/image helpers

**How to Run:**
1. Set Azure secrets
2. `dotnet run` in `Vision-03MEAI-AOAI`

**Tags:** #Vision #AzureOpenAI #ImageAnalysis

---

## Vision-04MEAI-AOAI-Spectre

**Description:**
Vision demo with Spectre.Console for advanced CLI output and Azure OpenAI.

**Dependencies:**
- Azure.AI.OpenAI: Azure OpenAI
- Spectre.Console: CLI output
- OpenCvSharp4: Image processing

**Main Features:**
- Vision with CLI output
- Video/image helpers

**How to Run:**
1. Set Azure secrets
2. `dotnet run` in `Vision-04MEAI-AOAI-Spectre`

**Tags:** #Vision #AzureOpenAI #Spectre #CLI

---

## Audio-01-SpeechMic

**Description:**
Audio demo using Microsoft Cognitive Services Speech for speech-to-text.

**Dependencies:**
- Microsoft.CognitiveServices.Speech: Speech-to-text

**Main Features:**
- Microphone input
- Real-time transcription

**How to Run:**
1. Set up Azure Speech secrets
2. `dotnet run` in `Audio-01-SpeechMic`

**Tags:** #Audio #SpeechToText #AzureSpeech

---

## Audio-02-RealTimeAudio

**Description:**
Real-time audio demo using NAudio and Azure OpenAI.

**Dependencies:**
- Azure.AI.OpenAI: Azure OpenAI
- NAudio: Audio input

**Main Features:**
- Real-time audio capture
- LLM integration

**How to Run:**
1. Set up secrets
2. `dotnet run` in `Audio-02-RealTimeAudio`

**Tags:** #Audio #RealTime #AzureOpenAI

---

## ImageGeneration-01

**Description:**
Image generation demo using Azure OpenAI DALL-E.

**Dependencies:**
- Azure.AI.OpenAI: DALL-E

**Main Features:**
- Generate images from prompts

**How to Run:**
1. Set Azure secrets
2. `dotnet run` in `ImageGeneration-01`

**Tags:** #ImageGeneration #AzureOpenAI #DALL-E

---

## VideoGeneration-AzureSora-01

**Description:**
Video generation demo (Azure Sora, basic config).

**Dependencies:**
- Microsoft.Extensions.Configuration.UserSecrets: Secrets

**Main Features:**
- Video generation (requires Azure Sora access)

**How to Run:**
1. Set up secrets
2. `dotnet run` in `VideoGeneration-AzureSora-01`

**Tags:** #VideoGeneration #AzureSora

---

## VideoGeneration-AzureSoraSDK-02

**Description:**
Video generation demo using Azure Sora SDK and .NET 8 hosting.

**Dependencies:**
- AzureSoraSDK: Azure Sora
- Microsoft.Extensions.*: Hosting, DI, logging

**Main Features:**
- Video generation with SDK

**How to Run:**
1. Set up secrets
2. `dotnet run` in `VideoGeneration-AzureSoraSDK-02`

**Tags:** #VideoGeneration #AzureSora #SDK

---

## AgentLabs-01-Simple

**Description:**
Simple Azure AI Agents demo using Persistent Agents and Projects SDKs.

**Dependencies:**
- Azure.AI.Agents.Persistent: Agents
- Azure.AI.Projects: Project orchestration
- Azure.Identity: Auth

**Main Features:**
- Persistent agent demo

**How to Run:**
1. Set up Azure secrets
2. `dotnet run` in `AgentLabs-01-Simple`

**Tags:** #Agents #AzureAI #Persistent

---

## AgentLabs-02-Functions

**Description:**
Azure AI Agents demo with function calling.

**Dependencies:**
- Azure.AI.Agents.Persistent: Agents
- Azure.AI.Projects: Project orchestration
- Azure.Identity: Auth

**Main Features:**
- Agent with function calling

**How to Run:**
1. Set up Azure secrets
2. `dotnet run` in `AgentLabs-02-Functions`

**Tags:** #Agents #AzureAI #Functions

---

## AgentLabs-03-OpenAPIs

**Description:**
Azure AI Agents demo with OpenAPI integration.

**Dependencies:**
- Azure.AI.Agents.Persistent: Agents
- Azure.AI.Projects: Project orchestration
- Azure.Identity: Auth

**Main Features:**
- Agent with OpenAPI function calling
- Includes OpenAPI spec

**How to Run:**
1. Set up Azure secrets
2. `dotnet run` in `AgentLabs-03-OpenAPIs`

**Tags:** #Agents #AzureAI #OpenAPI

---

## AIToolkit-01-SK-Chat

**Description:**
Chat demo using Semantic Kernel and AI Toolkit.

**Dependencies:**
- Microsoft.SemanticKernel: LLM orchestration

**Main Features:**
- Chat with plugins

**How to Run:**
1. Set up secrets
2. `dotnet run` in `AIToolkit-01-SK-Chat`

**Tags:** #SemanticKernel #AIToolkit #Chat

---

## AIToolkit-02-MEAI-Chat

**Description:**
Chat demo using Microsoft.Extensions.AI and OpenAI.

**Dependencies:**
- Microsoft.Extensions.AI: Core AI
- Microsoft.Extensions.AI.OpenAI: OpenAI integration

**Main Features:**
- Chat with OpenAI

**How to Run:**
1. Set up secrets
2. `dotnet run` in `AIToolkit-02-MEAI-Chat`

**Tags:** #AIToolkit #OpenAI #Chat

---

## DockerModels-01-SK-Chat

**Description:**
Chat demo using Semantic Kernel in Dockerized environment.

**Dependencies:**
- Microsoft.SemanticKernel: LLM orchestration

**Main Features:**
- Run in Docker
- Chat with plugins

**How to Run:**
1. Build Docker image (see lesson guide)
2. Run container

**Tags:** #SemanticKernel #Docker #Chat

---

## DockerModels-02-MEAI-Chat

**Description:**
Chat demo using Microsoft.Extensions.AI and OpenAI in Docker.

**Dependencies:**
- Microsoft.Extensions.AI: Core AI
- Microsoft.Extensions.AI.OpenAI: OpenAI integration

**Main Features:**
- Run in Docker
- Chat with OpenAI

**How to Run:**
1. Build Docker image (see lesson guide)
2. Run container

**Tags:** #AIToolkit #Docker #OpenAI #Chat

---

## OpenAI-FileProcessing-Pdf-01

**Description:**
Demo for processing PDF files with OpenAI and Semantic Kernel.

**Dependencies:**
- Microsoft.SemanticKernel: LLM orchestration
- Microsoft.SemanticKernel.Connectors.OpenAI: OpenAI connector

**Main Features:**
- PDF file processing
- Semantic Kernel plugins

**How to Run:**
1. Place PDFs in `docs/`
2. `dotnet run` in `OpenAI-FileProcessing-Pdf-01`

**Tags:** #OpenAI #SemanticKernel #PDF #FileProcessing

---

## MEAIFunctions

**Description:**
Demo for using Microsoft.Extensions.AI and AzureAIInference for function calling.

**Dependencies:**
- Microsoft.Extensions.AI: Core AI
- Microsoft.Extensions.AI.AzureAIInference: Azure OpenAI

**Main Features:**
- Function calling with Azure OpenAI

**How to Run:**
1. Set up Azure secrets
2. `dotnet run` in `MEAIFunctions`

**Tags:** #MEAI #AzureOpenAI #Functions

---

## MEAIFunctionsAzureOpenAI

**Description:**
Azure OpenAI function calling demo.

**Dependencies:**
- Azure.AI.OpenAI: Azure OpenAI
- Microsoft.Extensions.AI: Core AI
- Microsoft.Extensions.AI.AzureAIInference: Azure OpenAI
- Microsoft.Extensions.AI.OpenAI: OpenAI

**Main Features:**
- Function calling with Azure OpenAI

**How to Run:**
1. Set up Azure secrets
2. `dotnet run` in `MEAIFunctionsAzureOpenAI`

**Tags:** #AzureOpenAI #MEAI #Functions

---

## MEAIFunctionsOllama

**Description:**
Ollama function calling demo.

**Dependencies:**
- Microsoft.Extensions.AI: Core AI
- Microsoft.Extensions.AI.Ollama: Ollama integration

**Main Features:**
- Function calling with Ollama

**How to Run:**
1. Start Ollama
2. `dotnet run` in `MEAIFunctionsOllama`

**Tags:** #Ollama #MEAI #Functions

---

## MEAIVectorsShared

**Description:**
Shared vector data library for RAG demos.

**Dependencies:**
- Microsoft.Extensions.VectorData.Abstractions: Vector store

**Main Features:**
- Shared vector logic for RAG projects

**Tags:** #Vectors #RAG #Shared

---

*For screenshots, see the `images/` folder in this lesson or run the sample apps to generate your own.*
