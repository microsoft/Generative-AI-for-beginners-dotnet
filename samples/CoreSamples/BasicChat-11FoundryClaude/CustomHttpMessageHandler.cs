using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;

public class CustomHttpMessageHandler : DelegatingHandler
{
    public required string AzureClaudeDeploymentUrl { get; set; }
    public required string ApiKey { get; set; }
    public required string Model { get; set; }

    public CustomHttpMessageHandler() : base(new HttpClientHandler())
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string[] urls = { "deployments/claude-haiku-4-5" };

        bool isClaudeRequest = false;

        // validate if request.RequestUri is not null and if any of the deployment names appear in the absolute URI
        if (request.RequestUri != null && urls.Any(url => request.RequestUri.AbsoluteUri.Contains(url)))
        {
            isClaudeRequest = true;

            // Transform OpenAI request to Claude format
            await TransformRequestToClaude(request, cancellationToken);

            // set request.RequestUri to a new Uri with the AzureClaudeDeploymentUrl
            request.RequestUri = new Uri(AzureClaudeDeploymentUrl);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
            Console.WriteLine($"[ERROR] {response.StatusCode}: {errorBody}");
        }

        // Transform Claude response back to OpenAI format if needed
        if (isClaudeRequest)
        {
            response = await TransformResponseToOpenAI(response, cancellationToken);
        }

        return response;
    }

    private async Task TransformRequestToClaude(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Read the OpenAI request body
        var openAIBody = await request.Content!.ReadAsStringAsync(cancellationToken);
        var openAIJson = JsonNode.Parse(openAIBody);

        // Extract messages from OpenAI format
        var messages = openAIJson!["messages"]?.AsArray();
        var claudeMessages = new JsonArray();
        string? systemMessage = null;

        // Convert OpenAI messages to Claude format
        if (messages != null)
        {
            foreach (var msg in messages)
            {
                var role = msg!["role"]?.ToString();
                var content = msg["content"]?.ToString() ?? "";

                // Extract system message separately - Claude uses it as a parameter, not in messages array
                if (role == "system")
                {
                    systemMessage = content;
                    continue;
                }

                // Skip assistant messages that appear before any user message
                // Claude requires messages to start with user role
                if (role == "assistant" && claudeMessages.Count == 0)
                {
                    // Store as system message instead
                    systemMessage = content;
                    continue;
                }

                // Skip messages with empty content (Claude requires non-empty content)
                if (!string.IsNullOrWhiteSpace(content))
                {
                    claudeMessages.Add(new JsonObject
                    {
                        ["role"] = role == "assistant" ? "assistant" : "user",
                        ["content"] = content
                    });
                }
            }
        }

        // Build Claude request body
        var claudeBody = new JsonObject
        {
            ["model"] = Model ?? "claude-haiku-4-5",
            ["messages"] = claudeMessages,
            ["max_tokens"] = openAIJson["max_tokens"]?.GetValue<int>() ?? 2048,
            ["stream"] = openAIJson["stream"]?.GetValue<bool>() ?? false
        };

        // Add system message if present
        if (!string.IsNullOrEmpty(systemMessage))
        {
            claudeBody["system"] = systemMessage;
        }

        if (openAIJson["temperature"] != null)
        {
            claudeBody["temperature"] = openAIJson["temperature"]!.GetValue<double>();
        }

        // Add thinking parameter (disabled by default)
        claudeBody["thinking"] = new JsonObject
        {
            ["type"] = "disabled"
        };

        // CRITICAL: Remove ALL existing authentication headers
        request.Headers.Remove("api-key");
        request.Headers.Remove("Authorization");

        // Remove from request content headers if present
        if (request.Content?.Headers != null)
        {
            var headersToRemove = request.Content.Headers.Where(h =>
                h.Key.Equals("api-key", StringComparison.OrdinalIgnoreCase) ||
                h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var header in headersToRemove)
            {
                request.Content.Headers.Remove(header.Key);
            }
        }

        // Claude in Azure AI Foundry uses x-api-key header (not Authorization: Bearer)
        // See: https://learn.microsoft.com/en-us/azure/ai-foundry/foundry-models/how-to/use-foundry-models-claude
        if (!string.IsNullOrEmpty(ApiKey))
        {
            request.Headers.TryAddWithoutValidation("x-api-key", ApiKey);
        }
        else
        {
            Console.WriteLine("[ERROR] ApiKey is null or empty!");
        }

        // Add required Anthropic version header
        request.Headers.TryAddWithoutValidation("anthropic-version", "2023-06-01");

        // Ensure correct Content-Type
        request.Content = new StringContent(claudeBody.ToJsonString(), Encoding.UTF8, "application/json");
    }

    private async Task<HttpResponseMessage> TransformResponseToOpenAI(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            return response;
        }

        var claudeBody = await response.Content.ReadAsStringAsync(cancellationToken);

        // Check if this is a streaming response
        if (response.Content.Headers.ContentType?.MediaType == "text/event-stream")
        {
            // For streaming, we need to transform SSE events from Claude to OpenAI format
            return await TransformStreamingResponse(response, claudeBody, cancellationToken);
        }

        var claudeJson = JsonNode.Parse(claudeBody);

        // Transform Claude response to OpenAI format
        var openAIResponse = new JsonObject
        {
            ["id"] = claudeJson!["id"]?.ToString() ?? Guid.NewGuid().ToString(),
            ["object"] = "chat.completion",
            ["created"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            ["model"] = "claude-haiku-4-5",
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

        var newResponse = new HttpResponseMessage(response.StatusCode)
        {
            Content = new StringContent(openAIResponse.ToJsonString(), Encoding.UTF8, "application/json")
        };

        return newResponse;
    }

    private async Task<HttpResponseMessage> TransformStreamingResponse(HttpResponseMessage response, string content, CancellationToken cancellationToken)
    {
        // Read the streaming response and transform Claude SSE events to OpenAI format
        var originalStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var pipeStream = new System.IO.Pipelines.Pipe();
        var writer = pipeStream.Writer;

        // Start a background task to transform the stream
        _ = Task.Run(async () =>
        {
            try
            {
                using var reader = new StreamReader(originalStream, Encoding.UTF8);
                string? line;

                while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
                {
                    // Claude sends SSE events in format: "event: <type>" and "data: <json>"
                    if (line.StartsWith("data: "))
                    {
                        var jsonData = line.Substring(6).Trim();

                        // Skip if it's [DONE] or empty
                        if (string.IsNullOrWhiteSpace(jsonData) || jsonData == "[DONE]")
                        {
                            continue;
                        }

                        try
                        {
                            var claudeEvent = JsonNode.Parse(jsonData);
                            var eventType = claudeEvent?["type"]?.ToString();

                            // Transform Claude streaming events to OpenAI format
                            if (eventType == "content_block_delta")
                            {
                                var delta = claudeEvent?["delta"];
                                var text = delta?["text"]?.ToString();

                                if (!string.IsNullOrEmpty(text))
                                {
                                    var openAIChunk = new JsonObject
                                    {
                                        ["id"] = claudeEvent?["message_id"]?.ToString() ?? Guid.NewGuid().ToString(),
                                        ["object"] = "chat.completion.chunk",
                                        ["created"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                                        ["model"] = "claude-haiku-4-5",
                                        ["choices"] = new JsonArray
                                        {
                                            new JsonObject
                                            {
                                                ["index"] = 0,
                                                ["delta"] = new JsonObject
                                                {
                                                    ["content"] = text
                                                },
                                                ["finish_reason"] = JsonValue.Create<string?>(null)
                                            }
                                        }
                                    };

                                    var chunkData = $"data: {openAIChunk.ToJsonString()}\n\n";
                                    await writer.WriteAsync(Encoding.UTF8.GetBytes(chunkData), cancellationToken);
                                }
                            }
                            else if (eventType == "message_stop")
                            {
                                // Send final chunk with finish_reason
                                var finalChunk = new JsonObject
                                {
                                    ["id"] = Guid.NewGuid().ToString(),
                                    ["object"] = "chat.completion.chunk",
                                    ["created"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                                    ["model"] = "claude-haiku-4-5",
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

                                var finalData = $"data: {finalChunk.ToJsonString()}\n\ndata: [DONE]\n\n";
                                await writer.WriteAsync(Encoding.UTF8.GetBytes(finalData), cancellationToken);
                            }
                        }
                        catch (JsonException)
                        {
                            // Skip malformed JSON
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Streaming error: {ex.Message}");
            }
            finally
            {
                await writer.CompleteAsync();
            }
        }, cancellationToken);

        var newResponse = new HttpResponseMessage(response.StatusCode)
        {
            Content = new StreamContent(pipeStream.Reader.AsStream())
        };
        newResponse.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/event-stream");

        return newResponse;
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
}

