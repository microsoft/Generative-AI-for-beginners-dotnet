# Generative AI for Beginners .NET - A Course

## Practical lessons teaching you how to build Generative AI applications in .NET

[![GitHub license](https://img.shields.io/github/license/microsoft/Generative-AI-For-beginners-dotnet.svg)](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/blob/main/LICENSE)
[![GitHub contributors](https://img.shields.io/github/contributors/microsoft/Generative-AI-For-Beginners-Dotnet.svg)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/graphs/contributors/)
[![GitHub issues](https://img.shields.io/github/issues/microsoft/Generative-AI-For-Beginners-Dotnet.svg)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/issues/)
[![GitHub pull-requests](https://img.shields.io/github/issues-pr/microsoft/Generative-AI-For-Beginners-Dotnet.svg)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/pulls/)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com)

[![GitHub watchers](https://img.shields.io/github/watchers/microsoft/Generative-AI-For-Beginners-Dotnet.svg?style=social&label=Watch)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/watchers/)
[![GitHub forks](https://img.shields.io/github/forks/microsoft/Generative-AI-For-Beginners-Dotnet.svg?style=social&label=Fork)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/network/)
[![GitHub stars](https://img.shields.io/github/stars/microsoft/Generative-AI-For-Beginners-Dotnet.svg?style=social&label=Star)](https://github.com/microsoft/Generative-AI-For-Beginners-Dotnet/stargazers/)

[![Azure AI Community Discord](https://img.shields.io/discord/1113626258182504448?label=Azure%20AI%20Community%20Discord)](https://aka.ms/ai-discord/dotnet)

[![Microsoft Foundry GitHub Discussions](https://img.shields.io/badge/Discussions-Microsoft%20Foundry-blueviolet?logo=github&style=for-the-badge)](https://aka.ms/ai-discussions/dotnet)

![Generative AI for Beginners .NET logo](./images/main-logo.jpg)

Welcome to **Generative AI for Beginners .NET**, the hands-on course for .NET developers diving into the world of Generative AI!

This isn’t your typical “here’s some theory, good luck” course. This repository is all about **real-world applications** and **live coding** to empower .NET developers to take full advantage of Generative AI.

This is **hands-on**, **practical**, and designed to be **fun**!

Don't forget to [star (🌟) this repo](https://docs.github.com/en/get-started/exploring-projects-on-github/saving-repositories-with-stars) to find it easier later.

➡️Get your own copy by [Forking this repo](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/fork) and find it next in your own repositories.

## ✨ What's New

We're constantly improving this course with the latest AI tools, models, and practical samples:

- **🌐 NEW! AG-UI Integration Sample**

  Check out our new Agent Framework sample demonstrating **AG-UI** (Agent Gateway User Interface) - the distributed agent architecture pattern! This sample shows how to:
  - Separate AI agent backend from Blazor frontend for better scalability
  - Use `AGUIChatClient` to communicate with remote agents over HTTP
  - Implement RAG with semantic search and PDF document ingestion
  - Orchestrate distributed applications with .NET Aspire
  
  👉 [Explore the AG-UI Sample](./samples/AgentFx/AgentFx-AIWebChatApp-AG-UI/README.md)

- **🤖 Microsoft Agent Framework (AgentFx) Lesson**

  We've added a comprehensive new lesson on the Microsoft Agent Framework - the next evolution in .NET AI development! Learn how to:
  - Build orchestrated AI agent systems with specialized roles
  - Create multi-model workflows combining GitHub Models, Microsoft Foundry, and Ollama
  - Integrate with Model Context Protocol (MCP) for enhanced capabilities
  - Design sequential and concurrent agent workflows for complex tasks
  
  👉 [Explore Lesson 06: Microsoft Agent Framework](./06-AgentFx/readme.md)

[View all previous updates in our What's New archive](./10-WhatsNew/readme.md)

## 🚀 Introduction

Generative AI is transforming software development, and .NET is no exception. This course aims to simplify the journey by offering:

- Short 5-10 minute videos for each lesson.
- Fully functional .NET code samples you can run and explore.
- Integration with tools like **GitHub Codespaces** and **GitHub Models** for seamless setup and fast time-to-code. But if you want to run the samples locally with your own models, you can totally do that too.

You'll learn how to implement Generative AI into .NET projects, from basic text generation to building full-fledged solutions using **GitHub Models**, **Azure OpenAI Services** and **local models with Ollama**.

## 📦 Each Lesson Includes

- **Short Video**: A quick overview of the lesson (5-10 minutes).
- **Complete Code Samples**: Fully functional and ready to run.
- **Step-by-Step Guidance**: Simple instructions to help you learn and implement the concepts.
- **Deep Dive References**: This course focuses on the practical implementation of GenAI, to get deeper into the theoretical we also provide links to explanations in [Generative AI for Beginners - A Course](https://github.com/microsoft/generative-ai-for-beginners) when needed.

## 🗃️ Lessons

| #   | **Lesson Link** | **Description** |
| --- | --- | --- |
| 01  | [**Intro to Generative AI Basics for .NET Developers**](./01-IntroToGenAI/readme.md) | <ul><li>Overview of generative models and their applications in .NET</li></ul> |
| 02  | [**Setting Up for .NET Development with Generative AI**](./02-SetupDevEnvironment/readme.md) | <ul><li>Using libraries like **Microsoft.Extensions.AI** and **Semantic Kernel**.</li><li>Setup providers like GitHub Models, Microsoft Foundry, and local development like Ollama.</li></ul> |
| 03  | [**Core Generative AI Techniques with .NET**](./03-CoreGenerativeAITechniques/readme.md) | <ul><li>Text generation and conversational flows.</li><li> Multimodal capabilities (vision and audio).</li><li>Agents</li></ul> |
| 04  | [**Practical .NET Generative AI Samples**](./04-PracticalSamples/readme.md) | <ul><li>Complete samples demonstrating GenAI in real-life scenarios</li><li>Semantic search applications.</li><li>Multiple agent applications</li></ul> |
| 05  | [**.NET Apps created using GenAI tools (aka: Vibe Coding Prompts)**](./05-AppCreatedWithGenAI/readme.md) | <ul><li>Sample .NET apps generated using Generative AI tools like GitHub Copilot Agent.</li><li>First sample: **Retro Invaders retro console game 👾**</li></ul> |
| 06  | [**Microsoft Agent Framework (AgentFx)**](./06-AgentFx/readme.md) | <ul><li>Build powerful orchestrated AI agent systems.</li><li>Multi-model agent workflows.</li><li>Integration with Model Context Protocol (MCP).</li></ul> |
| 07  | [**Responsible Use of Generative AI in .NET Apps**](./09-ResponsibleGenAI/readme.md) | <ul><li>Ethical considerations, bias mitigation, and secure implementations.</li></ul> |

## 🌐 Multi-Language Support

| Language             | Code | Link to Translated README                               | Last Updated |
|----------------------|------|---------------------------------------------------------|--------------|
| Chinese (Simplified) | zh   | [Chinese Translation](./translations/zh/README.md)      | 2025-06-24   |
| Chinese (Traditional)| tw   | [Chinese Translation](./translations/tw/README.md)      | 2025-06-24   |
| French               | fr   | [French Translation](./translations/fr/README.md)       | 2025-06-24   |
| Japanese             | ja   | [Japanese Translation](./translations/ja/README.md)     | 2025-06-24   |
| Korean               | ko   | [Korean Translation](./translations/ko/README.md)       | 2025-06-24   |
| Portuguese           | pt   | [Portuguese Translation](./translations/pt/README.md)   | 2025-06-24   |
| Spanish              | es   | [Spanish Translation](./translations/es/README.md)      | 2025-06-24   |
| German               | de   | [German Translation](./translations/de/README.md)       | 2025-06-24   |

> **Note:** All translations were updated to match the original content on **2025-06-24**. See [PR #161](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/pull/161) for details.

## 🛠️ What You Need

To get started, you'll need:

1. A **GitHub account** (free is fine!) to [fork this entire repo](https://github.com/microsoft/generative-ai-for-beginners-dotnet/fork) to your own GitHub account.

1. **GitHub Codespaces enabled** for instant coding environments. You can enable GitHub Codespaces in your repository settings. Learn more about GitHub Codespaces [here](https://docs.github.com/en/codespaces).

1. Create your copy by [Forking this repo](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/fork), or use the `Fork` button at the top of this page.

1. A basic understanding of **.NET development**. Learn more about .NET [here](https://dotnet.microsoft.com/learn/dotnet/what-is-dotnet).

And that's it.

We've designed this course to be as low-friction as possible. We make use of the following to help you get started quickly:

- **Run in GitHub Codespaces**: With one click, you'll get a pre-configured environment to test and explore the lessons.
- **Leverage GitHub Models**: Try out AI-powered demos hosted directly within this repo, we explain more in the lessons, as we go. *(If you want to learn more about GitHub Models, click [here](https://docs.github.com/github-models))*

Then when you're ready to expand we also have guides for:

- Upgrading to **Azure OpenAI Services** for scalable and enterprise-ready solutions.
- Using **Ollama** to run models locally on your hardware for enhanced privacy and control.

## 🤝 Want to Help?

Contributions are welcome! Here's how you can help:

- [Report issues](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new) or bugs in the repo.

- Improve existing code samples or add new ones, fork this repo and propose some changes!
- Suggest additional lessons or enhancements.
- Do you have suggestions or found spelling or code errors?, [create a pull request](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/compare)

Check the [CONTRIBUTING.MD](./CONTRIBUTING.MD) file for details on how to get involved.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](./LICENSE) file for details.

## 🌐 Other Courses

We have a lot of other content to help your learning journey. Check out:

<!-- CO-OP TRANSLATOR OTHER COURSES START -->
### Azure / Edge / MCP / Agents

[![AZD for Beginners](https://img.shields.io/badge/AZD%20for%20Beginners-0078D4?style=for-the-badge&labelColor=E5E7EB&color=0078D4)](https://github.com/microsoft/AZD-for-beginners?WT.mc_id=academic-105485-koreyst)
[![Edge AI for Beginners](https://img.shields.io/badge/Edge%20AI%20for%20Beginners-00B8E4?style=for-the-badge&labelColor=E5E7EB&color=00B8E4)](https://github.com/microsoft/edgeai-for-beginners?WT.mc_id=academic-105485-koreyst)
[![MCP for Beginners](https://img.shields.io/badge/MCP%20for%20Beginners-009688?style=for-the-badge&labelColor=E5E7EB&color=009688)](https://github.com/microsoft/mcp-for-beginners?WT.mc_id=academic-105485-koreyst)
[![AI Agents for Beginners](https://img.shields.io/badge/AI%20Agents%20for%20Beginners-00C49A?style=for-the-badge&labelColor=E5E7EB&color=00C49A)](https://github.com/microsoft/ai-agents-for-beginners?WT.mc_id=academic-105485-koreyst)

---

### Generative AI Series

[![Generative AI for Beginners](https://img.shields.io/badge/Generative%20AI%20for%20Beginners-8B5CF6?style=for-the-badge&labelColor=E5E7EB&color=8B5CF6)](https://github.com/microsoft/generative-ai-for-beginners?WT.mc_id=academic-105485-koreyst)
[![Generative AI (.NET)](https://img.shields.io/badge/Generative%20AI%20(.NET)-9333EA?style=for-the-badge&labelColor=E5E7EB&color=9333EA)](https://github.com/microsoft/Generative-AI-for-beginners-dotnet?WT.mc_id=academic-105485-koreyst)
[![Generative AI (Java)](https://img.shields.io/badge/Generative%20AI%20(Java)-C084FC?style=for-the-badge&labelColor=E5E7EB&color=C084FC)](https://github.com/microsoft/generative-ai-for-beginners-java?WT.mc_id=academic-105485-koreyst)
[![Generative AI (JavaScript)](https://img.shields.io/badge/Generative%20AI%20(JavaScript)-E879F9?style=for-the-badge&labelColor=E5E7EB&color=E879F9)](https://github.com/microsoft/generative-ai-with-javascript?WT.mc_id=academic-105485-koreyst)

---

### Core Learning

[![ML for Beginners](https://img.shields.io/badge/ML%20for%20Beginners-22C55E?style=for-the-badge&labelColor=E5E7EB&color=22C55E)](https://aka.ms/ml-beginners?WT.mc_id=academic-105485-koreyst)
[![Data Science for Beginners](https://img.shields.io/badge/Data%20Science%20for%20Beginners-84CC16?style=for-the-badge&labelColor=E5E7EB&color=84CC16)](https://aka.ms/datascience-beginners?WT.mc_id=academic-105485-koreyst)
[![AI for Beginners](https://img.shields.io/badge/AI%20for%20Beginners-A3E635?style=for-the-badge&labelColor=E5E7EB&color=A3E635)](https://aka.ms/ai-beginners?WT.mc_id=academic-105485-koreyst)
[![Cybersecurity for Beginners](https://img.shields.io/badge/Cybersecurity%20for%20Beginners-F97316?style=for-the-badge&labelColor=E5E7EB&color=F97316)](https://github.com/microsoft/Security-101?WT.mc_id=academic-96948-sayoung)
[![Web Dev for Beginners](https://img.shields.io/badge/Web%20Dev%20for%20Beginners-EC4899?style=for-the-badge&labelColor=E5E7EB&color=EC4899)](https://aka.ms/webdev-beginners?WT.mc_id=academic-105485-koreyst)
[![IoT for Beginners](https://img.shields.io/badge/IoT%20for%20Beginners-14B8A6?style=for-the-badge&labelColor=E5E7EB&color=14B8A6)](https://aka.ms/iot-beginners?WT.mc_id=academic-105485-koreyst)
[![XR Development for Beginners](https://img.shields.io/badge/XR%20Development%20for%20Beginners-38BDF8?style=for-the-badge&labelColor=E5E7EB&color=38BDF8)](https://github.com/microsoft/xr-development-for-beginners?WT.mc_id=academic-105485-koreyst)

---

### Copilot Series

[![Copilot for AI Paired Programming](https://img.shields.io/badge/Copilot%20for%20AI%20Paired%20Programming-FACC15?style=for-the-badge&labelColor=E5E7EB&color=FACC15)](https://aka.ms/GitHubCopilotAI?WT.mc_id=academic-105485-koreyst)
[![Copilot for C#/.NET](https://img.shields.io/badge/Copilot%20for%20C%23/.NET-FBBF24?style=for-the-badge&labelColor=E5E7EB&color=FBBF24)](https://github.com/microsoft/mastering-github-copilot-for-dotnet-csharp-developers?WT.mc_id=academic-105485-koreyst)
[![Copilot Adventure](https://img.shields.io/badge/Copilot%20Adventure-FDE68A?style=for-the-badge&labelColor=E5E7EB&color=FDE68A)](https://github.com/microsoft/CopilotAdventures?WT.mc_id=academic-105485-koreyst)
<!-- CO-OP TRANSLATOR OTHER COURSES END -->

[Let's start learning Generative AI and .NET!](02-SetupDevEnvironment/readme.md) 🚀

## Getting Help

If you get stuck or have any questions about building AI apps, join:

[![Microsoft Foundry Discord](https://img.shields.io/badge/Discord-Microsoft_Foundry_Community_Discord-blue?style=for-the-badge&logo=discord&color=5865f2&logoColor=fff)](https://aka.ms/foundry/discord)

If you have product feedback or errors while building visit:

[![Microsoft Foundry Developer Forum](https://img.shields.io/badge/GitHub-Microsoft_Foundry_Developer_Forum-blue?style=for-the-badge&logo=github&color=000000&logoColor=fff)](https://aka.ms/foundry/forum)
