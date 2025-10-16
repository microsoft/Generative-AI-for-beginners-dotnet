# BasicChat-01MEAI - GitHub Models Chat Application

This is a simple chat application that demonstrates how to use Microsoft.Extensions.AI (MEAI) with GitHub Models.

## Prerequisites

- .NET 9.0 or later
- A GitHub Personal Access Token with access to GitHub Models
- The token should be set as an environment variable named `GITHUB_TOKEN`

## Current Model

This application uses the **Phi-4-mini-instruct** model, which is currently available in GitHub Models.

> **Note**: If you encounter an error about an unknown model (e.g., `Phi-3.5-MoE-instruct`), it means your code is outdated. The Phi-3.5 series models were deprecated in September 2025. Please ensure you're using the latest version of this repository.

## How to Run

1. Set up your GitHub token as an environment variable:
   ```bash
   export GITHUB_TOKEN="your_github_token_here"
   ```

2. Navigate to this directory:
   ```bash
   cd 02-SetupDevEnvironment/src/BasicChat-01MEAI
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

## Troubleshooting

### Error: Unknown model

If you see an error like:
```
Azure.RequestFailedException: Unknown model: /Phi-3.5-MoE-instruct
Status: 404 (Not Found)
```

This means:
- Your code references a deprecated model
- Pull the latest changes from the main repository
- Verify that `Program.cs` contains `"Phi-4-mini-instruct"` as the model name

### Error: Missing GITHUB_TOKEN

If you see:
```
Missing GITHUB_TOKEN environment variable
```

Make sure you've set up your GitHub Personal Access Token as described in the [setup guide](../../readme.md).

## Model Information

The current model configuration in `Program.cs`:

```csharp
IChatClient client = new ChatCompletionsClient(
        endpoint: new Uri("https://models.github.ai/inference"),
        new AzureKeyCredential(Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? throw new InvalidOperationException("Missing GITHUB_TOKEN environment variable. Ensure you followed the instructions to setup a GitHub Token to use GitHub Models.")))
        .AsIChatClient("Phi-4-mini-instruct");
```

## Available Models

For the latest list of available models in GitHub Models, visit:
- [GitHub Models Marketplace](https://github.com/marketplace?type=models)
- [GitHub Models Documentation](https://docs.github.com/en/github-models)

## Learn More

- [Main Setup Guide](../../readme.md)
- [Microsoft.Extensions.AI Documentation](https://learn.microsoft.com/dotnet/ai/)
- [GitHub Models Documentation](https://docs.github.com/en/github-models/prototyping-with-ai-models)
