# Quick Start Instructions

## 🚀 Running Single-File AI Demos

### Prerequisites
1. **.NET 9+ SDK** installed
2. **GitHub Token** with access to GitHub Models

### Setup
```bash
# Set your GitHub token (choose one method):

# Option 1: Environment variable (recommended)
export GITHUB_TOKEN="your_github_token_here"

# Option 2: User secrets (for development)
dotnet user-secrets set "GITHUB_TOKEN" "your_github_token_here"
```

### Run Demos
```bash
# Make script executable (first time only)
chmod +x run-demo.sh

# Run any demo
./run-demo.sh 01-llm-completion.cs
./run-demo.sh 02-chat-flow.cs
./run-demo.sh 03-functions-and-plugins.cs
./run-demo.sh 04-retrieval-augmented-generation.cs
./run-demo.sh 05-structured-output.cs
./run-demo.sh 06-multimodal.cs

# List available demos
./run-demo.sh
```

### What These Demos Show

| Demo | Concept | Key Learning |
|------|---------|--------------|
| `01-llm-completion.cs` | Text completions | Basic AI prompting and responses |
| `02-chat-flow.cs` | Interactive chat | Conversation memory and streaming |
| `03-functions-and-plugins.cs` | Tool calling | Extending AI with custom functions |
| `04-retrieval-augmented-generation.cs` | RAG patterns | AI + knowledge bases |
| `05-structured-output.cs` | JSON responses | Type-safe AI data extraction |
| `06-multimodal.cs` | Vision AI | Image analysis capabilities |

### Troubleshooting

**Authentication Error?**
- Ensure your `GITHUB_TOKEN` has access to GitHub Models
- Check that the token is properly set in environment variables

**Build Issues?**
- Make sure .NET 9+ SDK is installed
- The helper script automatically manages dependencies

**No .NET 10?**
- These demos work on .NET 9+ using the provided script
- When .NET 10 is released, you can use `dotnet run file.cs` directly

### Next Steps
- Experiment with different prompts and parameters
- Combine techniques from multiple demos
- Check out full applications in [Lesson 04 - Practical Samples](../../04-PracticalSamples/readme.md)

---

Happy coding with .NET and AI! 🎉