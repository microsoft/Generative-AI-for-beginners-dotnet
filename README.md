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

[![Azure AI Foundry GitHub Discussions](https://img.shields.io/badge/Discussions-Azure%20AI%20Foundry-blueviolet?logo=github&style=for-the-badge)](https://aka.ms/ai-discussions/dotnet)

![Generative AI for Beginners .NET logo](./images/main-logo.jpg)

Welcome to **Generative AI for Beginners .NET**, the hands-on course for .NET developers diving into the world of Generative AI!

This isn’t your typical “here’s some theory, good luck” course. This repository is all about **real-world applications** and **live coding** to empower .NET developers to take full advantage of Generative AI.

This is **hands-on**, **practical**, and designed to be **fun**!

Don't forget to [star (🌟) this repo](https://docs.github.com/en/get-started/exploring-projects-on-github/saving-repositories-with-stars) to find it easier later.

➡️Get your own copy by [Forking this repo](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/fork) and find it next in your own repositories.

## ✨ What's New

We're constantly improving this course with the latest AI tools, models, and practical samples:

- **� New! Basic Chat App for OpenAI gpt-oss Model!**

  Try out our new [Basic Chat application](./03-CoreGenerativeAITechniques/src/BasicChat-07Ollama-gpt-oss) designed to test the [OpenAI gpt-oss model](https://openai.com/index/introducing-gpt-oss/). This sample demonstrates how to use the latest open-source model from OpenAI in a .NET console app, making it easy to experiment with conversational AI locally or in the cloud.

- **🖼️ Hugging Face MCP Server Image Generation Samples!**

  Explore new C# console app samples that show how to use the Hugging Face MCP Server to generate images directly from your code.

  - [Try the sample using GitHub Models or Azure AI Foundry](./03-CoreGenerativeAITechniques/src/MCP-01-HuggingFace/Program.cs)
  - [Use Ollama for local model inference](./03-CoreGenerativeAITechniques/src/MCP-02-HuggingFace-Ollama/Program.cs)

- **🕹️ Apps Generated with AI**

  We're excited to introduce a new section featuring full applications generated with AI tools like GitHub Copilot Agent. These apps demonstrate how generative AI can be used to build real-world .NET solutions leveraging AI as a co-pilot.

  - **First Sample: [SpaceAINet Console App](./05-AppCreatedWithGenAI/SpaceAINet/README.md)**
  **SpaceAINet** is an AI-powered Space Battle game for .NET 9, designed to showcase how modern AI models can play classic games. The solution allows you to run the game with either local AI models (via Ollama) or cloud-based models (via Azure AI Foundry), which analyze the game state and predict the next best action to win.
  Try it out and see how AI can master classic arcade gameplay!
  - **New Sample: [Console GPU Viewer](./05-AppCreatedWithGenAI/ConsoleGpuViewer/README.md)**
  A lightweight console utility that demonstrates GPU diagnostics and how to integrate local model runners or visual tooling into console-based .NET apps. Useful for testing GPU availability before running local inference models.

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
| 02  | [**Setting Up for .NET Development with Generative AI**](./02-SetupDevEnvironment/readme.md) | <ul><li>Using libraries like **Microsoft.Extensions.AI** and **Semantic Kernel**.</li><li>Setup providers like GitHub Models, Azure AI Foundry, and local development like Ollama.</li></ul> |
| 03  | [**Core Generative AI Techniques with .NET**](./03-CoreGenerativeAITechniques/readme.md) | <ul><li>Text generation and conversational flows.</li><li> Multimodal capabilities (vision and audio).</li><li>Agents</li></ul> |
| 04  | [**Practical .NET Generative AI Samples**](./04-PracticalSamples/readme.md) | <ul><li>Complete samples demonstrating GenAI in real-life scenarios</li><li>Semantic search applications.</li><li>Multiple agent applications</li></ul> |
| 05  | [**.NET Apps created using GenAI tools (aka: Vibe Coding Prompts)**](./05-AppCreatedWithGenAI/readme.md) | <ul><li>Sample .NET apps generated using Generative AI tools like GitHub Copilot Agent.</li><li>First sample: **Retro Invaders retro console game 👾**</li></ul> |
| 06  | [**Responsible Use of Generative AI in .NET Apps**](./09-ResponsibleGenAI/readme.md) | <ul><li>Ethical considerations, bias mitigation, and secure implementations.</li></ul> |

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

- [Generative AI for Beginners](https://aka.ms/genai-beginners)
- [Generative AI for Beginners .NET](https://aka.ms/genainet)
- [Generative AI with JavaScript](https://aka.ms/genai-js-course)
- [Generative AI with Java](https://github.com/microsoft/Generative-AI-for-beginners-java)
- [AI for Beginners](https://aka.ms/ai-beginners)
- [AI Agents for Beginners - A Course](https://aka.ms/ai-agents-beginners)
- [Data Science for Beginners](https://aka.ms/datascience-beginners)
- [ML for Beginners](https://aka.ms/ml-beginners)
- [Cybersecurity for Beginners](https://github.com/microsoft/Security-101)
- [Web Dev for Beginners](https://aka.ms/webdev-beginners)
- [IoT for Beginners](https://aka.ms/iot-beginners)
- [XR Development for Beginners](https://github.com/microsoft/xr-development-for-beginners)
- [Mastering GitHub Copilot for Paired Programming](https://github.com/microsoft/Mastering-GitHub-Copilot-for-Paired-Programming)
- [Mastering GitHub Copilot for C#/.NET Developers](https://github.com/microsoft/mastering-github-copilot-for-dotnet-csharp-developers)
- [Choose Your Own Copilot Adventure](https://github.com/microsoft/CopilotAdventures)
- [Phi Cookbook: Hands-On Examples with Microsoft's Phi Models](https://aka.ms/phicookbook)

[Let's start learning Generative AI and .NET!](02-SetupDevEnvironment/readme.md) 🚀
