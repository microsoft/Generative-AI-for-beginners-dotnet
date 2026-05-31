# MAF-ImageGen-03-Foundry

A **Microsoft Agent Framework (MAF)** agent that generates images with **GPT-Image-2** on
**Microsoft Foundry**.

This sample mirrors the **Pitch Image Agent** from the *Zava Support Center* demo
(MAF + A2A + NVIDIA NeMo). A MAF `AIAgent` is given an **image tool** that wraps
[`ElBruno.Text2Image.Foundry`](https://www.nuget.org/packages/ElBruno.Text2Image.Foundry)'s
`GptImage2Generator`. The agent decides when to call the tool, generates the image, and
saves it as a PNG.

## Building blocks

| Block | Type | Auth |
|-------|------|------|
| The agent's brain | `IChatClient` (Microsoft.Extensions.AI) | keyless (`az login`) |
| The image tool | `GptImage2Generator` (ElBruno.Text2Image.Foundry / GPT-Image-2) | API key |

The chat model runs **keyless** (Microsoft Entra ID via `az login`). GPT-Image-2 image
generation uses **key auth**, so an API key is required.

## Secrets

```bash
dotnet user-secrets set "AzureOpenAI:Endpoint" "https://<your-endpoint>.services.ai.azure.com/"
dotnet user-secrets set "AzureOpenAI:Deployment" "gpt-5-mini"     # chat model (keyless)
dotnet user-secrets set "AzureOpenAI:ApiKey" "<your-api-key>"     # GPT-Image-2 (key auth)
```

Optional (defaults shown):

```bash
dotnet user-secrets set "AzureOpenAI:ImageDeployment" "gpt-image-2"
dotnet user-secrets set "AzureOpenAI:ImageModelName" "GPT-Image-2"
```

Then sign in for the keyless chat client:

```bash
az login
```

> The endpoint can be the bare Foundry resource URL or include an `/openai...` suffix —
> the sample normalizes it to the bare URL that `GptImage2Generator` expects.

## Run

```bash
# Uses the built-in incident-hero prompt (the Zava cold-open image)
dotnet run

# Or pass your own prompt
dotnet run -- "a corgi astronaut floating over Mars, comic style"
```

Generated PNGs are saved to the `images/` folder next to the built executable. The agent
prints the saved file path when it finishes.

## How it works

1. A `GptImage2Generator` is created against your Foundry endpoint + `gpt-image-2` deployment.
2. A local function `GenerateImage(prompt)` is wrapped as an `AIFunction` and handed to the
   agent via `chatClient.AsAIAgent(..., tools: [AIFunctionFactory.Create(GenerateImage)])`.
3. `agent.RunAsync(request)` lets the model infer a vivid prompt and call the tool, which
   generates the image and writes the PNG.
