using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

/// <summary>
/// HTTP message handler that transforms requests/responses between OpenAI format and Claude (Anthropic) format
/// for Microsoft Foundry deployments.
/// </summary>
public class ClaudeToOpenAIMessageHandler : DelegatingHandler
{
    // Constants for better maintainability
    private const string ClaudeAnthropicVersion = "2023-06-01";
    private const string ClaudeEventTypeContentDelta = "content_block_delta";
    private const string ClaudeEventTypeMessageStop = "message_stop";
    private const string OpenAIStreamDoneMarker = "[DONE]";
    private const int DefaultMaxTokens = 2048;
    private const string DefaultModelName = "claude-haiku-4-5";

    // Deployment URL patterns to detect Claude requests
    private static readonly string[] ClaudeDeploymentPatterns = {
        "deployments/claude-haiku",
        "deployments/claude-sonnet",
        "deployments/claude-opus"
    };

    /// <summary>
    /// Gets or sets the Azure Claude deployment URL endpoint.
    /// Format: https://{resource-name}.services.ai.azure.com/anthropic/v1/messages
    /// </summary>
    public required string AzureClaudeDeploymentUrl { get; set; }

    /// <summary>
    /// Gets or sets the API key for authenticating with Claude in Microsoft Foundry.
    /// </summary>
    public required string ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the Claude model name (e.g., "claude-haiku-4-5", "claude-sonnet-4-5").
    /// </summary>
    public required string Model { get; set; }

    public ClaudeToOpenAIMessageHandler() : base(new HttpClientHandler())
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        bool isClaudeRequest = IsClaudeRequest(request);

        if (isClaudeRequest)
        {
            await TransformRequestToClaude(request, cancellationToken);
            request.RequestUri = new Uri(AzureClaudeDeploymentUrl);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await LogErrorResponse(response, cancellationToken);
        }

        if (isClaudeRequest)
        {
            response = await TransformResponseToOpenAI(response, cancellationToken);
        }

        return response;
    }

    private bool IsClaudeRequest(HttpRequestMessage request)
    {
        return request.RequestUri != null &&
               ClaudeDeploymentPatterns.Any(pattern => request.RequestUri.AbsoluteUri.Contains(pattern));
    }

    private async Task LogErrorResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
        Console.WriteLine($"[ERROR] {response.StatusCode}: {errorBody}");
    }

    #region Request Transformation

    private async Task TransformRequestToClaude(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var openAIJson = await ReadOpenAIRequestBody(request, cancellationToken);
        var (claudeMessages, systemMessage) = ConvertMessagesToClaudeFormat(openAIJson);
        var claudeBody = BuildClaudeRequestBody(openAIJson, claudeMessages, systemMessage);

        ConfigureClaudeRequestHeaders(request);
        request.Content = new StringContent(claudeBody.ToJsonString(), Encoding.UTF8, "application/json");
    }

    private async Task<JsonNode> ReadOpenAIRequestBody(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var openAIBody = await request.Content!.ReadAsStringAsync(cancellationToken);
        return JsonNode.Parse(openAIBody)!;
    }

    private (JsonArray messages, string? systemMessage) ConvertMessagesToClaudeFormat(JsonNode openAIJson)
    {
        var messages = openAIJson["messages"]?.AsArray();
        var claudeMessages = new JsonArray();
        string? systemMessage = null;

        if (messages != null)
        {
            foreach (var msg in messages)
            {
                ProcessMessage(msg!, claudeMessages, ref systemMessage);
            }
        }

        return (claudeMessages, systemMessage);
    }

    private void ProcessMessage(JsonNode msg, JsonArray claudeMessages, ref string? systemMessage)
    {
        var role = msg["role"]?.ToString();
        var content = msg["content"]?.ToString() ?? "";

        if (ShouldExtractAsSystemMessage(role, claudeMessages))
        {
            systemMessage = content;
            return;
        }

        if (ShouldSkipMessage(content))
        {
            return;
        }

        claudeMessages.Add(CreateClaudeMessage(role, content));
    }

    private bool ShouldExtractAsSystemMessage(string? role, JsonArray claudeMessages)
    {
        // Extract system messages or initial assistant messages as system prompts
        return role == "system" || (role == "assistant" && claudeMessages.Count == 0);
    }

    private bool ShouldSkipMessage(string content)
    {
        // Claude requires non-empty content
        return string.IsNullOrWhiteSpace(content);
    }

    private JsonObject CreateClaudeMessage(string? role, string content)
    {
        return new JsonObject
        {
            ["role"] = role == "assistant" ? "assistant" : "user",
            ["content"] = content
        };
    }

    private JsonObject BuildClaudeRequestBody(JsonNode openAIJson, JsonArray claudeMessages, string? systemMessage)
    {
        var claudeBody = new JsonObject
        {
            ["model"] = Model ?? DefaultModelName,
            ["messages"] = claudeMessages,
            ["max_tokens"] = openAIJson["max_tokens"]?.GetValue<int>() ?? DefaultMaxTokens,
            ["stream"] = openAIJson["stream"]?.GetValue<bool>() ?? false
        };

        if (!string.IsNullOrEmpty(systemMessage))
        {
            claudeBody["system"] = systemMessage;
        }

        if (openAIJson["temperature"] != null)
        {
            claudeBody["temperature"] = openAIJson["temperature"]!.GetValue<double>();
        }

        // Claude thinking parameter (disabled by default)
        claudeBody["thinking"] = new JsonObject { ["type"] = "disabled" };

        return claudeBody;
    }

    private void ConfigureClaudeRequestHeaders(HttpRequestMessage request)
    {
        RemoveOpenAIAuthHeaders(request);
        AddClaudeAuthHeaders(request);
    }

    private void RemoveOpenAIAuthHeaders(HttpRequestMessage request)
    {
        request.Headers.Remove("api-key");
        request.Headers.Remove("Authorization");

        if (request.Content?.Headers != null)
        {
            var headersToRemove = request.Content.Headers
                .Where(h => h.Key.Equals("api-key", StringComparison.OrdinalIgnoreCase) ||
                           h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var header in headersToRemove)
            {
                request.Content.Headers.Remove(header.Key);
            }
        }
    }

    private void AddClaudeAuthHeaders(HttpRequestMessage request)
    {
        // Claude in Microsoft Foundry uses x-api-key header (not Authorization: Bearer)
        // See: https://learn.microsoft.com/en-us/azure/ai-foundry/foundry-models/how-to/use-foundry-models-claude
        if (!string.IsNullOrEmpty(ApiKey))
        {
            request.Headers.TryAddWithoutValidation("x-api-key", ApiKey);
        }
        else
        {
            Console.WriteLine("[ERROR] ApiKey is null or empty!");
        }

        request.Headers.TryAddWithoutValidation("anthropic-version", ClaudeAnthropicVersion);
    }

    #endregion

    #region Response Transformation

    private async Task<HttpResponseMessage> TransformResponseToOpenAI(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            return response;
        }

        var contentType = response.Content.Headers.ContentType?.MediaType;

        if (contentType == "text/event-stream")
        {
            return await TransformStreamingResponse(response, cancellationToken);
        }

        return await TransformNonStreamingResponse(response, cancellationToken);
    }

    private async Task<HttpResponseMessage> TransformNonStreamingResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var claudeBody = await response.Content.ReadAsStringAsync(cancellationToken);
        var claudeJson = JsonNode.Parse(claudeBody);

        var openAIResponse = new JsonObject
        {
            ["id"] = claudeJson!["id"]?.ToString() ?? Guid.NewGuid().ToString(),
            ["object"] = "chat.completion",
            ["created"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ["model"] = DefaultModelName,
            ["choices"] = new JsonArray
            {
                new JsonObject
                {
                    ["index"] = 0,
                    ["message"] = new JsonObject
                    {
                        ["role"] = "assistant",
                        ["content"] = ExtractClaudeContent(claudeJson["content"])
                    },
                    ["finish_reason"] = claudeJson["stop_reason"]?.ToString() ?? "stop"
                }
            }
        };

        return new HttpResponseMessage(response.StatusCode)
        {
            Content = new StringContent(openAIResponse.ToJsonString(), Encoding.UTF8, "application/json")
        };
    }

    private string ExtractClaudeContent(JsonNode? contentNode)
    {
        if (contentNode is JsonArray contentArray && contentArray.Count > 0)
        {
            var firstContent = contentArray[0];
            if (firstContent?["type"]?.ToString() == "text")
            {
                return firstContent["text"]?.ToString() ?? "";
            }
        }
        return contentNode?.ToString() ?? "";
    }

    #endregion

    #region Streaming Response Transformation

    private async Task<HttpResponseMessage> TransformStreamingResponse(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var originalStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var transformedStream = CreateTransformedStreamPipeline();

        StartStreamTransformationTask(originalStream, transformedStream, cancellationToken);

        return CreateStreamingResponse(response, transformedStream);
    }

    private System.IO.Pipelines.Pipe CreateTransformedStreamPipeline()
    {
        return new System.IO.Pipelines.Pipe();
    }

    private void StartStreamTransformationTask(
        Stream originalStream,
        System.IO.Pipelines.Pipe pipeStream,
        CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            await TransformClaudeStreamToOpenAI(originalStream, pipeStream.Writer, cancellationToken);
        }, cancellationToken);
    }

    private async Task TransformClaudeStreamToOpenAI(
        Stream originalStream,
        System.IO.Pipelines.PipeWriter writer,
        CancellationToken cancellationToken)
    {
        try
        {
            using var reader = new StreamReader(originalStream, Encoding.UTF8);
            await ProcessClaudeSseEvents(reader, writer, cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Streaming error: {ex.Message}");
        }
        finally
        {
            await writer.CompleteAsync();
        }
    }

    private async Task ProcessClaudeSseEvents(
        StreamReader reader,
        System.IO.Pipelines.PipeWriter writer,
        CancellationToken cancellationToken)
    {
        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            if (line.StartsWith("data: "))
            {
                await ProcessClaudeDataEvent(line, writer, cancellationToken);
            }
        }
    }

    private async Task ProcessClaudeDataEvent(
        string line,
        System.IO.Pipelines.PipeWriter writer,
        CancellationToken cancellationToken)
    {
        var jsonData = line.Substring(6).Trim();

        if (ShouldSkipDataEvent(jsonData))
        {
            return;
        }

        try
        {
            var claudeEvent = JsonNode.Parse(jsonData);
            var eventType = claudeEvent?["type"]?.ToString();

            await TransformAndWriteEvent(eventType, claudeEvent, writer, cancellationToken);
        }
        catch (JsonException)
        {
            // Skip malformed JSON silently
        }
    }

    private bool ShouldSkipDataEvent(string jsonData)
    {
        return string.IsNullOrWhiteSpace(jsonData) || jsonData == OpenAIStreamDoneMarker;
    }

    private async Task TransformAndWriteEvent(
        string? eventType,
        JsonNode? claudeEvent,
        System.IO.Pipelines.PipeWriter writer,
        CancellationToken cancellationToken)
    {
        switch (eventType)
        {
            case ClaudeEventTypeContentDelta:
                await WriteContentDeltaChunk(claudeEvent, writer, cancellationToken);
                break;
            case ClaudeEventTypeMessageStop:
                await WriteFinalChunk(writer, cancellationToken);
                break;
        }
    }

    private async Task WriteContentDeltaChunk(
        JsonNode? claudeEvent,
        System.IO.Pipelines.PipeWriter writer,
        CancellationToken cancellationToken)
    {
        var text = ExtractDeltaText(claudeEvent);

        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var openAIChunk = CreateOpenAIStreamingChunk(claudeEvent, text, finishReason: null);
        await WriteChunkToStream(openAIChunk, writer, cancellationToken);
    }

    private string? ExtractDeltaText(JsonNode? claudeEvent)
    {
        return claudeEvent?["delta"]?["text"]?.ToString();
    }

    private JsonObject CreateOpenAIStreamingChunk(JsonNode? claudeEvent, string text, string? finishReason)
    {
        return new JsonObject
        {
            ["id"] = claudeEvent?["message_id"]?.ToString() ?? Guid.NewGuid().ToString(),
            ["object"] = "chat.completion.chunk",
            ["created"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ["model"] = DefaultModelName,
            ["choices"] = new JsonArray
            {
                new JsonObject
                {
                    ["index"] = 0,
                    ["delta"] = new JsonObject { ["content"] = text },
                    ["finish_reason"] = JsonValue.Create(finishReason)
                }
            }
        };
    }

    private async Task WriteChunkToStream(
        JsonObject chunk,
        System.IO.Pipelines.PipeWriter writer,
        CancellationToken cancellationToken)
    {
        var chunkData = $"data: {chunk.ToJsonString()}\n\n";
        await writer.WriteAsync(Encoding.UTF8.GetBytes(chunkData), cancellationToken);
    }

    private async Task WriteFinalChunk(System.IO.Pipelines.PipeWriter writer, CancellationToken cancellationToken)
    {
        var finalChunk = CreateFinalChunk();
        var finalData = $"data: {finalChunk.ToJsonString()}\n\ndata: {OpenAIStreamDoneMarker}\n\n";
        await writer.WriteAsync(Encoding.UTF8.GetBytes(finalData), cancellationToken);
    }

    private JsonObject CreateFinalChunk()
    {
        return new JsonObject
        {
            ["id"] = Guid.NewGuid().ToString(),
            ["object"] = "chat.completion.chunk",
            ["created"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ["model"] = DefaultModelName,
            ["choices"] = new JsonArray
            {
                new JsonObject
                {
                    ["index"] = 0,
                    ["delta"] = new JsonObject(),
                    ["finish_reason"] = "stop"
                }
            }
        };
    }

    private HttpResponseMessage CreateStreamingResponse(
        HttpResponseMessage originalResponse,
        System.IO.Pipelines.Pipe pipeStream)
    {
        var newResponse = new HttpResponseMessage(originalResponse.StatusCode)
        {
            Content = new StreamContent(pipeStream.Reader.AsStream())
        };
        newResponse.Content.Headers.ContentType =
            new System.Net.Http.Headers.MediaTypeHeaderValue("text/event-stream");

        return newResponse;
    }

    #endregion
}
