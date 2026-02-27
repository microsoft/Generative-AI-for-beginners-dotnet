# Setting Up: Local Development with Ollama

Run AI models locally on your machine. This is free, private, and works offline.

## 1. Install Ollama
1. Download Ollama from [ollama.com/download](https://ollama.com/download).
2. Install it on your machine (Windows, macOS, or Linux).
3. Verify it's running by opening a terminal and typing:
   ```bash
   ollama --version
   ```

## 2. Pull the Models
You need to download the "brain" for the AI to work. For this workshop, we use **Phi-4** (a powerful Small Language Model) and **All-MiniLM** (for embeddings).

Open your terminal and run:

```bash
# Pull the main chat model
ollama pull phi4-mini

# Pull the embedding model (used later for RAG)
ollama pull all-minilm
```
*Note: These downloads may take a few minutes depending on your internet speed.*

## 3. Configure Your Project
When you run the .NET samples in this course, they are pre-configured to look for Ollama at `http://localhost:11434`.

If Ollama is running in your system tray, you are good to go!

[⬅️ Back to Introduction](./readme.md)
