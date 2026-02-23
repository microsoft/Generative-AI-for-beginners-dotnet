# What's New in Generative AI for Beginners .NET

This page tracks the history of new features, tools, and models added to the course. Check back for updates!

## February 2026

### **🔄 Architecture Update: Migration from Semantic Kernel to Microsoft.Extensions.AI (MEAI)**

We've completed a major architectural upgrade to standardize on **Microsoft.Extensions.AI (MEAI)** as our primary AI abstraction layer. This migration ensures better alignment with .NET's modern AI ecosystem and improves long-term maintainability.

**What Changed:**
- **11 pure Semantic Kernel samples** have been moved to `samples/deprecated/` for historical reference
- **MEAI is now the standard** for all new samples and lessons
- **Lesson documentation updated** to guide learners toward MEAI-based samples
- **Mixed projects (BasicChat-05, BasicChat-11)** have had SK dependencies removed while maintaining MEAI
- **NuGet packages aligned** across 74 projects with rc→GA upgrades

**For Learners:**
- All active samples now use MEAI for AI operations
- Deprecated SK samples are still available if you need them
- Lesson 02 (Setup) and Lesson 03 (Core Techniques) have been updated with current best practices

[View detailed changelog →](../docs/changelog/2025-sk-deprecation-nuget-upgrades.md)

## January 2026

### **🆕 New! Basic Chat App for OpenAI gpt-oss Model**

Try out our new [Basic Chat application](../samples/CoreSamples/BasicChat-07Ollama-gpt-oss) designed to test the [OpenAI gpt-oss model](https://openai.com/index/introducing-gpt-oss/). This sample demonstrates how to use the latest open-source model from OpenAI in a .NET console app, making it easy to experiment with conversational AI locally or in the cloud.

### **🖼️ Hugging Face MCP Server Image Generation Samples**

Explore new C# console app samples that show how to use the Hugging Face MCP Server to generate images directly from your code.

- [Try the sample using GitHub Models or Microsoft Foundry](../samples/CoreSamples/MCP-01-HuggingFace/Program.cs)
- [Use Ollama for local model inference](../samples/CoreSamples/MCP-02-HuggingFace-Ollama/Program.cs)

### **🕹️ Apps Generated with AI - Console GPU Viewer**

We've added a new sample to the Apps Generated with AI section:

- **[Console GPU Viewer](../samples/AppsWithGenAI/ConsoleGpuViewer/README.md)**: A lightweight console utility that demonstrates GPU diagnostics and how to integrate local model runners or visual tooling into console-based .NET apps. Useful for testing GPU availability before running local inference models.

## July 2025

### **🕹️ New! Apps Generated with AI**

We're excited to introduce a new section featuring full applications generated with AI tools like GitHub Copilot Agent. These apps demonstrate how generative AI can be used to build real-world .NET solutions leveraging AI as a co-pilot.

- **First Sample: SpaceAINet**
  - [SpaceAINet Console App](../samples/AppsWithGenAI/SpaceAINet/README.md)
  - **SpaceAINet** is an AI-powered Space Battle game for .NET 9, designed to showcase how modern AI models can play classic games. The solution allows you to run the game with either local AI models (via Ollama) or cloud-based models (via Microsoft Foundry), which analyze the game state and predict the next best action to win.
  - Try it out and see how AI can master classic arcade gameplay!

## June 2025

### New Samples and Demos

- **New: Foundry Local demos!**
  - Lesson 3 now features hands-on demos for [Foundry Local models](https://github.com/microsoft/Foundry-Local/tree/main).
  - See the official docs: [Foundry Local documentation](https://learn.microsoft.com/azure/ai-foundry/foundry-local/)
  - **Full explanation and code samples are available in [03-CoreGenerativeAITechniques/06-LocalModelRunners.md](./03-CoreGenerativeAITechniques/06-LocalModelRunners.md)**

- **New: Azure OpenAI Sora Video Generation Demo!**
  - Lesson 3 now features a hands-on demo showing how to generate videos from text prompts using the new [Sora video generation model](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation) in Azure OpenAI.
  - The sample demonstrates how to:
    - Submit a video generation job with a creative prompt.
    - Poll for job status and download the resulting video file automatically.
    - Save the generated video to your desktop for easy viewing.
  - See the official docs: [Azure OpenAI Sora video generation](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)
  - Find the sample in [Lesson 3: Core Generative AI Techniques /src/VideoGeneration-AzureSora-01/Program.cs](../samples/CoreSamples/VideoGeneration-AzureSora-01/Program.cs)

- **New: Azure OpenAI Image Generation Model (`gpt-image-1`)**: Lesson 3 now features code samples for using the new Azure OpenAI image generation model, `gpt-image-1`. Learn how to generate images from .NET using the latest Azure OpenAI capabilities.
  - See the official: [How to use Azure OpenAI image generation models](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1) and [openai-dotnet image generation guide](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images) for more details.
  - Find the sample in [Lesson 3: Core Generative AI Techniques .. /src/ImageGeneration-01.csproj](../samples/CoreSamples/ImageGeneration-01/ImageGeneration-01.csproj).

- **New Scenario: Concurrent Agent Orchestration in eShopLite**: The [eShopLite repository](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/07-AgentsConcurrent) now features a scenario demonstrating concurrent agent orchestration using Semantic Kernel. This scenario showcases how multiple agents can work in parallel to analyze user queries and provide valuable insights for future analysis.

## May 2025

### Azure OpenAI Image Generation Model: gpt-image-1

- **New Lesson 3 Sample: Azure OpenAI Image Generation**
  - Lesson 3 now includes code samples and explanations for using the new Azure OpenAI image generation model: `gpt-image-1`.
  - Learn how to generate images using the latest Azure OpenAI capabilities directly from .NET.
  - See the [official Azure OpenAI DALL·E documentation](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1) and [openai-dotnet image generation guide](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images) for more details.
  - Find the sample in [Lesson 3: Core Generative AI Techniques](../03-CoreGenerativeAITechniques/).

### Run Local Models with AI Toolkit and Docker

- **New: Run Local Models with AI Toolkit and Docker**: Explore new samples for running models locally using [AI Toolkit for Visual Studio Code](https://code.visualstudio.com/docs/intelligentapps/overview) and [Docker Model Runner](https://docs.docker.com/model-runner/). The source code is in [../samples/CoreSamples/](../samples/CoreSamples/) and demonstrates how to use Semantic Kernel and Microsoft Extensions for AI to interact with these models.

## March 2025

### MCP Library Integration

- **Model Context Protocol for .NET**: We've added support for the [MCP C# SDK](https://github.com/modelcontextprotocol/csharp-sdk), which provides a standardized way to communicate with AI models across different providers.
- This integration enables more consistent interactions with models while reducing provider lock-in.
- Check out our new samples demonstrating MCP integration in the [Core Generative AI Techniques](../03-CoreGenerativeAITechniques/) section.

### eShopLite Scenarios Repository

- **New eShopLite Repository**: All eShopLite scenarios are now available in a single repository: [https://aka.ms/eshoplite/repo](https://aka.ms/eshoplite/repo)
- The new repo includes:
  - Product catalog browsing
  - Shopping cart management
  - Order placement and tracking
  - User authentication and profiles
  - Integration with Generative AI for recommendations and chat
  - Admin dashboard for product and order management
