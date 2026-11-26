# 15. Image decisions with Llama App

Use the community-maintained [ElBruno.AI.Decisions.Llama](https://www.nuget.org/packages/ElBruno.AI.Decisions.Llama) package to send a local image to a vision-capable decision model in [Llama App](https://llama.app/docs/decision-models). The model returns a typed choice and probabilities, not a generated image description.

## Requirements

- .NET 10 SDK
- Llama App running with an installed image-capable decision model
- A PNG, JPEG, or WebP image no larger than 8 MiB

From the repository root, provide a path to an image:

```powershell
dotnet run --project .\samples\CoreSamples\Decision-15-LlamaImage -- .\path\to\photo.jpg
```

The default model ID is `LiquidAI/d1-omni-600M-GGUF:Q8_0`. If you installed another compatible model, pass its ID after the image path:

```powershell
dotnet run --project .\samples\CoreSamples\Decision-15-LlamaImage -- .\path\to\photo.jpg "your-installed-model-id"
```

Llama App's default endpoint is `http://localhost:9931/`. The selected model must support image decisions. The sample sends image bytes to the configured endpoint and never logs or saves them. Use only a trusted endpoint, especially if changing the default local endpoint. The model's probabilities are not calibrated guarantees of visual accuracy.
