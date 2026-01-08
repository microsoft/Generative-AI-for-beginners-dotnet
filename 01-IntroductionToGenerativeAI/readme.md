# Introduction to Generative AI for .NET Developers

[![Introduction to Generative AI](./images/LIM_GAN_01_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson1-genaireview)

_Click the image to watch the video_

---

## You Already Know How to Build AI Applications

Here's something that might surprise you: if you've ever called a REST API, you already have the core skill needed to build AI-powered applications.

Think about it. When you call an API, you send a request and get a response. Building with AI is the same pattern:

```csharp
var response = await chatClient.GetResponseAsync("Summarize this customer feedback");

Console.WriteLine(response);
```

That's the entire concept. You send a prompt. You get a response. Your existing .NET skills handle everything else: dependency injection, configuration, error handling, async patterns.

No PhD required. No Python. No complex ML pipelines.

This course will prove it to you, not with slides and theory, but with real, runnable code that you'll build yourself.

---

## What You Will Learn in This Lesson

By the end of this lesson, you will:

1. **Understand what Generative AI actually is**, stripped of the hype and mystery
2. **Know why your .NET skills already prepare you** for AI development
3. **See the key difference** between traditional programming and AI-powered applications
4. **Get your development environment ready** so you can start coding immediately

No prerequisites beyond basic .NET knowledge. We'll build everything step by step.

---

## What Is Generative AI, Really?

**Generative AI is software that creates new content** (text, images, code, audio) based on patterns it learned from existing data.

That's the entire concept. When you use ChatGPT, GitHub Copilot, or DALL-E, you're using generative AI.

### How Does It Actually Work?

At the heart of most generative AI applications, usually, are **Large Language Models (LLMs)**. These are neural networks trained on massive amounts of text data. During training, they learn patterns, relationships, and structures in language.

When you send a prompt to an LLM, it doesn't "understand" in the human sense. Instead, it predicts the most likely next tokens (words or word pieces) based on patterns it learned. The result feels intelligent because those patterns came from billions of examples of human writing.

Key concepts:

- **Tokens**: LLMs break text into small pieces called tokens. A token might be a word, part of a word, or punctuation. When you see "token limits," this refers to how much text the model can process at once.

    ![How text is broken into tokens](./images/tokens-visualization.png)

    _Source: [Microsoft Learn - Prompt Engineering](https://learn.microsoft.com/azure/ai-services/openai/concepts/prompt-engineering)_

- **Context Window**: The amount of text an LLM can "see" at one time. Larger context windows let you include more information in your prompts.
- **Temperature**: A setting that controls randomness. Lower temperature (0.0-0.3) gives more predictable, focused responses. Higher temperature (0.7-1.0) gives more creative, varied responses.

### How It's Different from Traditional Programming

Here's the key mental shift:

| Traditional Programming | Generative AI |
|------------------------|---------------|
| You write explicit rules | You describe what you want |
| Output is **deterministic** (same input = same output) | Output is **probabilistic** (same input ≈ similar outputs) |
| `if (score > 90) return "A";` | `"Grade this essay and explain why"` |
| You handle every edge case | The model generalizes from patterns |

**Example in plain terms:**

- **Old way:** Write 500 lines of code to analyze sentiment in customer reviews using regex and keyword matching.
- **New way:** Send the review to an AI model with the prompt "Is this customer happy, neutral, or unhappy? Explain why."

It learned patterns from millions of examples and applies them to your specific input.

### What About Training Models?

Here's what you **don't** need to worry about:

- Training models (that's for research labs with massive compute budgets)
- Understanding neural network math (helpful but not required)
- Learning Python (.NET works great)

Here's what you **do** need to know:

- How to call AI models (just like calling any API)
- How to write good prompts (we'll cover this)
- How to integrate AI into your applications (your existing .NET skills)

### The Importance of Prompts

Since you're describing what you want rather than coding explicit rules, the way you write prompts matters enormously. A well-crafted prompt can be the difference between a useful response and a useless one.

Good prompts typically:

- **Provide context**: Tell the model what role it should play or what domain it's working in
- **Be specific**: Vague prompts get vague answers
- **Include examples**: Show the model what good output looks like
- **Set constraints**: Specify format, length, or style requirements

We'll practice prompt engineering throughout this course, starting in Lesson 02.

---

## Why .NET Developers Are Ready Right Now

There's a misconception that AI development requires Python or specialized ML expertise.

**This is false.**

You already have the core skills:

| Skill You Already Have | How It Applies to AI |
|-----------------------|---------------------|
| Calling REST APIs | AI models are accessed via APIs |
| Dependency Injection | Swap AI providers without changing code |
| Async/await patterns | AI calls are async operations |
| Configuration management | API keys, endpoints, model selection |
| Error handling | AI calls can fail, need retry logic |

### The Magic of `IChatClient`

Microsoft created a unified abstraction called `IChatClient` (part of Microsoft.Extensions.AI). It works just like the patterns you already know:

- Think `ILogger`, but for AI conversations
- Think `HttpClient`, but for model interactions

**One interface, any AI provider:**

```csharp
// Use OpenAI
IChatClient client = new OpenAIChatClient("gpt-4o", apiKey);

// Or use a local model with Ollama
IChatClient client = new OllamaChatClient(new Uri("http://localhost:11434"), "phi4-mini");

// Or use Azure OpenAI
IChatClient client = new AzureOpenAIChatClient(endpoint, credential, "gpt-4o");

// Your application code stays exactly the same!
var response = await client.GetResponseAsync("Hello, AI!");
```

Switch providers by changing one line. Your business logic never changes.

---

## The Tools You'll Use

Now that you understand the concepts, let's look at the tools. We'll go from simple to advanced.

### Layer 1: Microsoft.Extensions.AI (MEAI)

This is your foundation. MEAI provides:

- `IChatClient` for text conversations
- `IEmbeddingGenerator` for vector search scenarios
- Built-in support for caching, telemetry, and retries

**Think of it as:** The plumbing that connects your code to any AI model.

![Microsoft.Extensions.AI Architecture](./images/meai-architecture-diagram.png)

_Source: [Microsoft Extensions AI - Preview Announcement](https://devblogs.microsoft.com/dotnet/introducing-microsoft-extensions-ai-preview/)_

### Layer 2: AI Models (The "Brains")

You have options:

| Option | Best For | Cost |
|--------|----------|------|
| **Ollama (Local)** | Privacy, offline work, learning | Free |
| **GitHub Models** | Getting started quickly | Free tier |
| **Azure OpenAI** | Production, enterprise, compliance | Pay-per-use |

All of these work with the same `IChatClient` interface!

### Layer 3: Microsoft Agent Framework (Coming Later)

Once you're comfortable with basic AI calls, you'll learn to build **Agents**. AI workers that can use tools, maintain state, and collaborate with other agents.

But that's for later lessons. First, let's get you coding.

---

## Setting Up Your Development Environment

[![Watch the Video Tutorial](./images/LIM_GAN_02_thumb_w480.png)](https://aka.ms/genainnet/videos/lesson2-setupdevenv)

_Click the image to watch the setup video_

We have removed the setup barriers. Choose the path that fits your workflow:

### Path A: GitHub Codespaces (Recommended)
**Best for:** Beginners, or if you don't want to install anything.
*   **What you get:** A full cloud IDE with .NET 9 and tools pre-installed.
*   **Models:** Use **GitHub Models** (Free) or **Azure OpenAI**.
*   [**Access the Codespaces Setup Guide**](./setup-github-codespaces.md)

### Path B: Local Development (Ollama)
**Best for:** Privacy, offline work, and free local capability.
*   **What you get:** You run the "brain" on your own laptop.
*   **Models:** **Phi-4**, **Llama 3**, etc.
*   [**Access the Local Ollama Setup Guide**](./setup-local-ollama.md)

### Path C: Azure OpenAI (Enterprise)
**Best for:** Production scenarios and existing Azure users.
*   [**Access the Azure OpenAI Setup Guide**](./setup-azure-openai.md)

---

## What You Will Build in This Course

In this workshop, you will not just learn theory. You will build:

| What You'll Build | Description | Lesson |
|-------------------|-------------|--------|
| **Chat Applications** | Conversations with context and memory | Lesson 02: Generative AI Techniques |
| **Semantic Search** | Search that understands meaning, not just keywords | Lesson 03: AI Patterns and Applications |
| **RAG Applications** | Apps grounded in your own documents and data | Lesson 03: AI Patterns and Applications |
| **Tool-Using Agents** | Agents that call APIs and take actions | Lesson 04: AI Agents |
| **Multi-Agent Systems** | Autonomous agents that collaborate | Lesson 04: AI Agents |

Each lesson builds on the previous one, and everything is hands-on code you can run immediately.

---

## Let's Review: What You Learned

Before you move on, let's reinforce the key concepts:

| Concept | Key Takeaway |
|---------|-------------|
| **Generative AI** | Software that creates new content based on learned patterns |
| **Deterministic vs. Probabilistic** | Traditional code gives exact outputs; AI gives generated, variable outputs |
| **Your .NET skills transfer** | API calls, DI, async/await: you already know the patterns |
| **`IChatClient`** | One interface for any AI provider (OpenAI, Ollama, Azure) |
| **No training required** | You use pre-trained models; focus on integration |

### Quick Self-Check

Can you answer these questions?

1. What's the main difference between traditional programming and generative AI?
2. Why don't you need to train your own models?
3. What is `IChatClient` and why is it useful?

If you can answer all three, you're ready for the next lesson!

---

## Next Steps

You have two things to do:

1. **Set up your environment** using one of the paths above (we recommend GitHub Codespaces to start)
2. **Move to the next lesson** where you'll write your first real AI application

[**Start Coding: Core Generative AI Techniques**](../03-CoreGenerativeAITechniques/readme.md)

---

## Additional Resources

Want to go deeper? Here are some excellent resources:

**.NET and AI:**
- [Microsoft.Extensions.AI Documentation](https://learn.microsoft.com/dotnet/ai/ai-extensions)
- [Azure OpenAI Service Documentation](https://learn.microsoft.com/azure/ai-services/openai/)
- [Microsoft Agent Framework](https://learn.microsoft.com/agent-framework/)

**Running Models Locally:**
- [Ollama](https://ollama.com/)

**Want the Full Picture?**
- [Generative AI for Beginners](https://github.com/microsoft/generative-ai-for-beginners) - Our 21-lesson course covering GenAI concepts in depth (Python/TypeScript focus)

---

**You've completed Lesson 1!**

You now understand what generative AI is, why you're already equipped to build with it, and what tools you'll use. In the next lesson, you'll write real code and see AI respond to your prompts.

Let's build something amazing.
