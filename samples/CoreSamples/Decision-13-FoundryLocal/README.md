# 13. Typed decisions with Foundry Local

Use the community-maintained [ElBruno.AI.Decisions.Foundry.Local](https://www.nuget.org/packages/ElBruno.AI.Decisions.Foundry.Local) package to run choice, score, and assessment decisions on a cached local model. The sample uses the Foundry Local SDK directly. It does not call a chat-completions endpoint or generate explanatory paragraphs.

## Requirements

- .NET 11 SDK and .NET 10 runtime (the project targets .NET 10)
- Foundry Local installed
- A cached model that advertises typed decision support

Download the default model once, then run the sample from the repository root:

```powershell
foundry model download kev-0.8b
dotnet run --project .\samples\CoreSamples\Decision-13-FoundryLocal
```

The program reuses the Foundry Local CLI cache and does not download a model implicitly. To select another cached compatible model, pass its model ID:

```powershell
dotnet run --project .\samples\CoreSamples\Decision-13-FoundryLocal -- kev-4b
```

`kev-0.8b` has been verified with CPU inference. `kev-4b` advertises decision capabilities but has not been inference-verified. The resolver checks model capabilities and fails if the selected model does not support choice, score, and assessment operations.

## What to notice

The same synthetic support request is evaluated three ways:

- **Choose** selects a support route and returns a probability distribution.
- **Score** returns probabilities for ordered urgency levels and their expected zero-based rubric position.
- **Assess** estimates the probability of a proposition.

The model is resolved from the local cache, loaded for inference, and unloaded when the program exits. The sample does not delete cached model files. Model confidence and probabilities are not calibrated guarantees of correctness.
