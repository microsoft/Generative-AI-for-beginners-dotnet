# Introduction to Generative AI for .NET Developers

**Welcome to the future of .NET development. It looks a lot like the present, just smarter.**

[![Introduction to Generative AI](./images/LIM_GAN_01_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson1-genaireview)

_⬆️ Click the image to watch the video ⬆️_

## The Narrative: AI is Not Magic, It's Software

There is a misconception that Generative AI is a magical box, or something reserved only for Python developers and data scientists.

**This is false.**

As a .NET developer, you already have every skill needed to build powerful AI applications. Generative AI is simply a new type of API call, one that is **probabilistic** rather than **deterministic**.

*   **Deterministic Programming (Old 1.0):** You write `if (x == 10) return "High";`. The output is always exactly what you coded.
*   **Probabilistic AI (New 2.0):** You send a prompt: *"Analyze this data and give me a risk assessment."* The model returns a generated response based on patterns it has learned.

Your job isn't to train models (that's for research labs). Your job is to **orchestrate** them, integrate them into your apps, and ensure they solve real business problems. You know how to manage dependencies, handle HTTP requests, and architect systems. **You are ready.**

---

## The Microsoft AI Stack: Your New Toolbelt

We have unified the ecosystem. You no longer need to stitch together random libraries. We are focusing on a clean, composable stack designed for the enterprise.

![Agent Framework Architecture](https://learn.microsoft.com/en-us/agent-framework/media/agent.svg)

### 1. The Abstraction: Microsoft.Extensions.AI (MEAI)
Think of this like `ILogger` or dependency injection, but for AI. MEAI provides a unified interface (`IChatClient`) for interacting with *any* AI model.
*   Want to use **OpenAI**? Use `IChatClient`.
*   Want to use a local **Llama** model? Use `IChatClient`.
*   Want to switch providers halfway through a project? Change one line of configuration.

### 2. The Orchestration: Microsoft Agent Framework (AgentFx)
This is the unified foundation for building AI agents, created by the teams behind **Semantic Kernel** and **AutoGen**. It combines the enterprise strengths of Semantic Kernel (state management, telemetry) with the composability of AutoGen.

*   **Agents:** Individual units that use LLMs to process inputs, call tools, and generate responses.
*   **Workflows:** Graph-based connections that let multiple agents work together to solve complex tasks.

This is where you build your logic. Instead of just "chatting with a bot," you build **Agents**—specialized workers (e.g., a "Researcher" agent and a "Writer" agent) that collaborate to perform work.

### 3. The Models
The "brains" behind the operation.
*   **Local Models (Ollama):** Run models like Phi-4 or Llama 3 locally on your machine. Free, private, and works offline.
*   **Cloud Models (Azure OpenAI / GitHub Models):** Enterprise-scale models like GPT-4o. Secure, compliant, and massive scale.

---

## Setting Up Your Development Environment

[![Watch the Video Tutorial](./images/LIM_GAN_02_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson2-setupdevenv)

_⬆️ Click the image to watch the setup video ⬆️_

We have removed the setup barriers. Choose the path that fits your workflow:

### Path A: GitHub Codespaces (Recommended) ☁️
**Best for:** Beginners, or if you don't want to install anything.
*   **What you get:** A full cloud IDE with .NET 9 and tools pre-installed.
*   **Models:** Use **GitHub Models** (Free) or **Azure OpenAI**.
*   [**👉 Access the Codespaces Setup Guide**](./setup-github-codespaces.md)

### Path B: Local Development (Ollama) 🏠
**Best for:** Privacy, offline work, and free local capability.
*   **What you get:** You run the "brain" on your own laptop.
*   **Models:** **Phi-4**, **Llama 3**, etc.
*   [**👉 Access the Local Ollama Setup Guide**](./setup-local-ollama.md)

### Path C: Azure OpenAI (Enterprise) 🏢
**Best for:** Production scenarios and existing Azure users.
*   [**👉 Access the Azure OpenAI Setup Guide**](./setup-azure-openai.md)

---

## What You Will Build
In this workshop, you will not just learn theory. You will build:
*   Local AI chatbots that run offline.
*   Multi-agent systems where specialized AI workers collaborate.
*   Real-world applications integrating images, audio, and tools.

**You are not starting from zero. Your .NET skills transfer directly.**

[**➡️ Start Coding: Core Generative AI Techniques**](../03-CoreGenerativeAITechniques/readme.md)
