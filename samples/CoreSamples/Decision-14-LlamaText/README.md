# 14. Text decisions with Llama App

Use the community-maintained [ElBruno.AI.Decisions.Llama](https://www.nuget.org/packages/ElBruno.AI.Decisions.Llama) package to make local text decisions through [Llama App](https://llama.app/docs/decision-models). The sample demonstrates typed choice, score, and assessment results without generating a chat explanation.

## Requirements

- .NET 11 SDK and .NET 10 runtime (the project targets .NET 10)
- Llama App running with a locally installed decision model

The default model ID is `LiquidAI/d1-omni-600M-GGUF:Q8_0`. Check the model IDs available from your Llama App instance, then run the sample from the repository root:

```powershell
Invoke-RestMethod http://localhost:9931/v1/models
dotnet run --project .\samples\CoreSamples\Decision-14-LlamaText
```

To select another installed decision model, pass its model ID:

```powershell
dotnet run --project .\samples\CoreSamples\Decision-14-LlamaText -- "your-installed-model-id"
```

The provider defaults to `http://localhost:9931/`. Llama App and the model must already be running; the sample does not download models. The same synthetic request is used to classify a support route, score its urgency with a probability for each rubric level, and assess whether contact was requested. The reported score is the expected zero-based rubric position.

## What to notice

The result contains typed answers and probabilities. Confidence is a model estimate, not calibrated correctness. Evaluate decisions against representative examples and keep human review for uncertain or consequential outcomes.
