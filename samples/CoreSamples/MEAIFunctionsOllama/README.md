# Function calling with Ollama

Requires .NET 11 or later, a running Ollama server, and `ollama pull llama3.2`.

From the repository root:

```powershell
dotnet run --project .\samples\CoreSamples\MEAIFunctionsOllama
```

The arithmetic request is sent without tools, so it cannot invoke the weather function. The second request offers `GetTheWeather` through `UseFunctionInvocation()` and explicitly asks the model to call it once. Middleware permits at most three tool-invocation rounds, followed by one final request without tools. The weather response is limited to 128 tokens, so a model cannot loop through tool calls indefinitely. The overall request timeout remains two minutes.

Expect the invocation message, fixed rainy demo weather, an umbrella recommendation, and a positive `Weather tool calls` count. This is synthetic data, not a live forecast. Restricting available tools is an application boundary; a prompt alone is not an authorization mechanism.

`OLLAMA_ENDPOINT` and `OLLAMA_MODEL` override the defaults. Use a tool-capable model. If the model does not call the function, the sample fails explicitly. Small models can route tools incorrectly, so inspect the observed invocation as well as the final prose.

[Continue along the Ollama path](../../../docs/ollama-learning-path.md)
