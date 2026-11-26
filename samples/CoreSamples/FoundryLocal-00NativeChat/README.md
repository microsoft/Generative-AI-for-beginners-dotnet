# Native Foundry Local Hello World

Use the .NET 11 SDK and .NET 10 runtime; start with the [setup guide](../../../01-IntroductionToGenerativeAI/setup-foundry-local.md). From the repository root:

```powershell
dotnet run --project .\samples\CoreSamples\FoundryLocal-00NativeChat -- --prepare
dotnet run --project .\samples\CoreSamples\FoundryLocal-00NativeChat
```

`--prepare` explicitly permits a missing model download. Default `phi-4-mini` generates `Native answer: 4`. Without the flag, a missing model fails. This SDK-native example creates a `ChatSession`, sends a `Request`, reads text items, and unloads the model without deleting cached files. No listener, CLI, or cloud key is required.

The shared bootstrap prints the actual model ID and uses a private CPU-first runtime with nonessential telemetry disabled. Three-minute cancellation is cooperative; use the acceptance runner for a hard process watchdog. See [Program.cs](./Program.cs) and [the helper](../Common/FoundryCourseRuntime.cs).

Continue with [MEAI techniques](../FoundryLocal-01Techniques/README.md) and the [five-lesson route](../../../docs/foundry-local-learning-path.md).
