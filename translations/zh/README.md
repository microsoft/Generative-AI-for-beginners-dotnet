# 初学者的生成式 AI .NET - 一门课程

### 通过实践课程，学习如何在 .NET 中构建生成式 AI 应用

[![GitHub license](https://img.shields.io/github/license/microsoft/Generative-AI-For-beginners-dotnet.svg)](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/LICENSE)
[![GitHub contributors](https://img.shields.io/github/contributors/microsoft/Generative-AI-For-Beginners-Dotnet.svg)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/graphs/contributors/)
[![GitHub issues](https://img.shields.io/github/issues/microsoft/Generative-AI-For-Beginners-Dotnet.svg)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/issues/)
[![GitHub pull-requests](https://img.shields.io/github/issues-pr/microsoft/Generative-AI-For-Beginners-Dotnet.svg)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/pulls/)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)

[![GitHub watchers](https://img.shields.io/github/watchers/microsoft/Generative-AI-For-Beginners-Dotnet.svg?style=social&label=Watch)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/watchers/)
[![GitHub forks](https://img.shields.io/github/forks/microsoft/Generative-AI-For-Beginners-Dotnet.svg?style=social&label=Fork)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/network/)
[![GitHub stars](https://img.shields.io/github/stars/microsoft/Generative-AI-For-Beginners-Dotnet.svg?style=social&label=Star)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/stargazers/)


[![Azure AI Community Discord](https://img.shields.io/discord/1113626258182504448?label=Azure%20AI%20Community%20Discord)](https://aka.ms/ai-discord/dotnet)
[![Azure AI Foundry GitHub Discussions](https://img.shields.io/badge/Discussions-Azure%20AI%20Foundry-blueviolet?logo=github&style=for-the-badge)](https://aka.ms/ai-discussions/dotnet)

![生成式 AI 初学者 .NET 标志](../../translated_images/main-logo.5ac974278bc20b3520e631aaa6bf8799f2d219c5aec555da85555725546f25f8.zh.jpg)

欢迎来到 **生成式 AI 初学者 .NET**，这是为 .NET 开发者量身打造的实践课程，带你探索生成式 AI 的世界！

这不是一门普通的“理论讲解，祝你好运”类型的课程。本课程专注于 **实际应用** 和 **实时编码**，帮助 .NET 开发者充分利用生成式 AI 的潜力。

这是一门 **动手实践**、**注重实用性**，同时设计得 **有趣** 的课程！

别忘了 [为此项目加星标 (🌟)](https://docs.github.com/en/get-started/exploring-projects-on-github/saving-repositories-with-stars)，方便日后找到它。

➡️通过 [Fork 此项目](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/fork) 获取自己的副本，并在自己的仓库中轻松找到它。

## ✨ 最新更新！

我们不断改进本课程，加入最新的 AI 工具、模型和实践示例：

- **新增：Foundry Local 演示！**
  - 第3课现在提供 [Foundry Local 模型](https://github.com/microsoft/Foundry-Local/tree/main) 的实践演示。
  - 查看官方文档：[Foundry Local 文档](https://learn.microsoft.com/azure/ai-foundry/foundry-local/)
  - **完整说明和代码示例请参见 [03-CoreGenerativeAITechniques/06-LocalModelRunners.md](../../03-CoreGenerativeAITechniques/06-LocalModelRunners.md)**

- **新增：Azure OpenAI Sora 视频生成演示！**
  - 第3课现在提供实践演示，展示如何使用新的 [Sora 视频生成模型](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation) 在 Azure OpenAI 中从文本提示生成视频。
  - 该示例演示如何：
    - 使用创意提示提交视频生成任务。
    - 轮询任务状态并自动下载生成的视频文件。
    - 将生成的视频保存到桌面以便轻松查看。
  - 查看官方文档：[Azure OpenAI Sora 视频生成](https://learn.microsoft.com/azure/ai-services/openai/concepts/video-generation)
  - 在这里找到示例：[第3课：核心生成式 AI 技术 /src/VideoGeneration-AzureSora-01/Program.cs](../../samples/CoreSamples/VideoGeneration-AzureSora-01/Program.cs)

- **新增：Azure OpenAI 图像生成模型 (`gpt-image-1`)**：第3课现在提供使用新的 Azure OpenAI 图像生成模型 `gpt-image-1` 的代码示例。学习如何使用最新的 Azure OpenAI 功能从 .NET 生成图像。
  - 查看官方文档：[如何使用 Azure OpenAI 图像生成模型](https://learn.microsoft.com/azure/ai-services/openai/how-to/dall-e?tabs=gpt-image-1) 和 [openai-dotnet 图像生成指南](https://github.com/openai/openai-dotnet?tab=readme-ov-file#how-to-generate-images) 了解更多详情。
  - 在这里找到示例：[第3课：核心生成式 AI 技术 .. /src/ImageGeneration-01.csproj](../../samples/CoreSamples/ImageGeneration-01/ImageGeneration-01.csproj)。

- **新场景：eShopLite 中的并发代理编排**：[eShopLite 仓库](https://github.com/Azure-Samples/eShopLite/tree/main/scenarios/07-AgentsConcurrent) 现在提供一个场景，演示如何使用 Semantic Kernel 进行并发代理编排。此场景展示了多个代理如何并行工作来分析用户查询并为未来分析提供有价值的见解。

[在我们的最新更新部分查看所有之前的更新](./10-WhatsNew/readme.md)

## 🚀 课程简介

生成式 AI 正在改变软件开发的方式，而 .NET 也不例外。本课程旨在通过以下内容简化学习过程：

- 每节课包含 5-10 分钟的短视频。
- 提供完整的 .NET 代码示例，供你运行和探索。
- 集成 **GitHub Codespaces** 和 **GitHub Models** 等工具，快速设置环境并进入编码状态。如果你想在本地运行示例并使用自己的模型，也完全可以。

你将学习如何将生成式 AI 集成到 .NET 项目中，从基础的文本生成到使用 **GitHub Models**、**Azure OpenAI 服务** 和 **Ollama 的本地模型** 构建完整解决方案。

## 📦 每节课包含

- **短视频**：每节课的快速概览（5-10 分钟）。
- **完整代码示例**：功能齐全，随时可运行。
- **逐步指导**：简明的说明帮助你学习并实现相关概念。
- **深入参考**：本课程专注于生成式 AI 的实践实现，若需深入理论学习，我们还提供链接至 [生成式 AI 初学者 - 一门课程](https://github.com/microsoft/generative-ai-for-beginners) 的相关内容。

## 🗃️ 课程目录

| #   | **课程链接** | **课程描述** |
| --- | --- | --- |
| 01  | [**.NET 开发者的生成式 AI 基础入门**](./01-IntroToGenAI/readme.md) | <ul><li>概览生成模型及其在 .NET 中的应用</li></ul> |
| 02  | [**为生成式 AI 设置 .NET 开发环境**](./02-SetupDevEnvironment/readme.md) | <ul><li>使用 **Microsoft.Extensions.AI** 和 **Semantic Kernel** 等库。</li><li>配置 GitHub Models、Azure AI Foundry，以及 Ollama 等本地开发工具。</li></ul> |
| 03  | [**.NET 中的核心生成式 AI 技术**](./03-CoreGenerativeAITechniques/readme.md) | <ul><li>文本生成与对话流程。</li><li>多模态功能（视觉和音频）。</li><li>智能代理。</li></ul> |
| 04  | [**实用的 .NET 生成式 AI 示例**](./04-PracticalSamples/readme.md) | <ul><li>展示生成式 AI 在实际场景中的完整示例。</li><li>语义搜索应用。</li><li>多代理应用。</li></ul> |
| 05  | [**在 .NET 应用中负责任地使用生成式 AI**](./05-ResponsibleGenAI/readme.md) | <ul><li>伦理考量、偏见缓解及安全实现。</li></ul> |

## 🌐 多语言支持

| 语言                | 代码 | 翻译版 README 链接                                | 最后更新日期 |
|---------------------|------|--------------------------------------------------|--------------|
| 简体中文            | zh   | [中文翻译](./README.md)          | 2025-06-11   |
| 繁体中文            | tw   | [中文翻译](../tw/README.md)          | 2025-06-11   |
| 法语                | fr   | [法语翻译](../fr/README.md)          | 2025-06-11   |
| 日语                | ja   | [日语翻译](../ja/README.md)          | 2025-06-11   |
| 韩语                | ko   | [韩语翻译](../ko/README.md)          | 2025-06-11   |
| 葡萄牙语            | pt   | [葡萄牙语翻译](../pt/README.md)      | 2025-06-11   |
| 西班牙语            | es   | [西班牙语翻译](../es/README.md)      | 2025-06-11   |
| 德语                | de   | [德语翻译](../de/README.md)          | 2025-06-11   |

## 🛠️ 所需准备

开始学习前，你需要：

1. 一个 **GitHub 账号**（免费账号即可！）来 [Fork 整个项目](https://github.com/microsoft/generative-ai-for-beginners-dotnet/fork) 到你的 GitHub 账户。

1. 启用 **GitHub Codespaces** 以获得即时编码环境。你可以在仓库设置中启用 GitHub Codespaces。点击 [此处](https://docs.github.com/en/codespaces) 了解更多关于 GitHub Codespaces 的信息。

1. 创建副本，通过 [Fork 此项目](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/fork) 或点击页面顶部的 `Fork` 按钮。

1. 对 **.NET 开发** 有基本的了解。点击 [此处](https://dotnet.microsoft.com/learn/dotnet/what-is-dotnet) 了解更多关于 .NET 的信息。

就是这么简单。

我们设计了这门课程，力求降低学习门槛。我们使用以下工具帮助你快速入门：

- **在 GitHub Codespaces 中运行**：只需点击一下，即可获得预配置的环境，方便你测试和探索课程内容。
- **利用 GitHub 模型**：尝试直接在这个仓库中托管的 AI 驱动的演示，我们会在课程中逐步讲解更多内容。*(如果想了解更多关于 GitHub 模型的信息，请点击[这里](https://docs.github.com/github-models))*

当你准备好进一步探索时，我们还提供以下指南：

- 升级到 **Azure OpenAI Services**，以获得可扩展且适用于企业的解决方案。
- 使用 **Ollama** 在本地硬件上运行模型，增强隐私和控制。

## 🤝 想要贡献？

欢迎贡献！以下是你可以帮助的方法：

- [报告问题](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new) 或修复仓库中的错误。

- 改进现有代码示例或添加新示例，fork 本仓库并提交一些更改！
- 提议额外的课程或改进。
- 你有建议或发现拼写或代码错误吗？[创建一个 pull request](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/compare)

查看 [CONTRIBUTING.md](CONTRIBUTING.md) 文件，了解如何参与贡献。

## 📄 许可协议

本项目采用 MIT 许可协议 - 详情请参阅 [LICENSE](../../LICENSE) 文件。

## 🌐 其他课程

我们还有许多其他内容可以帮助你的学习之旅，查看以下资源：

- [生成式 AI 初学者教程](https://aka.ms/genai-beginners)
- [生成式 AI 初学者 .NET](https://aka.ms/genainet)
- [JavaScript 的生成式 AI](https://aka.ms/genai-js-course)
- [AI 初学者教程](https://aka.ms/ai-beginners)
- [AI 代理初学者课程](https://aka.ms/ai-agents-beginners)
- [数据科学初学者教程](https://aka.ms/datascience-beginners)
- [机器学习初学者教程](https://aka.ms/ml-beginners)
- [网络安全初学者教程](https://github.com/microsoft/Security-101) 
- [Web 开发初学者教程](https://aka.ms/webdev-beginners)
- [物联网初学者教程](https://aka.ms/iot-beginners)
- [XR 开发初学者教程](https://github.com/microsoft/xr-development-for-beginners)
- [精通 GitHub Copilot 进行配对编程](https://github.com/microsoft/Mastering-GitHub-Copilot-for-Paired-Programming)
- [C#/.NET 开发者的 GitHub Copilot 精通指南](https://github.com/microsoft/mastering-github-copilot-for-dotnet-csharp-developers)
- [选择你的 Copilot 探索之旅](https://github.com/microsoft/CopilotAdventures)
- [Phi Cookbook: 微软 Phi 模型实践示例](https://aka.ms/phicookbook)

[让我们开始学习生成式 AI 和 .NET 吧！](02-SetupDevEnvironment/readme.md) 🚀
本文件使用基于机器的人工智能翻译服务进行翻译。虽然我们尽力确保准确性，但请注意，自动翻译可能包含错误或不准确之处。应以原文所在的母语版本作为权威来源。对于关键信息，建议寻求专业人工翻译服务。因使用本翻译而导致的任何误解或误读，我们概不负责。