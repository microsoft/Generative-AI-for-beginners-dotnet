using System.Text.Json;

namespace FoundryLocal.Samples.Common;

public static class FoundryLocalSampleSupport
{
    public const string BaseUrlEnvVar = "FOUNDRY_LOCAL_BASE_URL";
    public const string ModelEnvVar = "FOUNDRY_LOCAL_MODEL";
    public const string ApiKeyEnvVar = "FOUNDRY_LOCAL_API_KEY";

    public const string DefaultBaseUrl = "http://127.0.0.1:5273/v1";
    public const string DefaultModel = "qwen2.5-0.5b";
    public const string DefaultApiKey = "local-dev-key";

    public static FoundryLocalSampleSettings LoadSettings()
    {
        var configuredBaseUrl = Environment.GetEnvironmentVariable(BaseUrlEnvVar);
        var configuredModel = Environment.GetEnvironmentVariable(ModelEnvVar);
        var configuredApiKey = Environment.GetEnvironmentVariable(ApiKeyEnvVar);

        configuredBaseUrl = string.IsNullOrWhiteSpace(configuredBaseUrl) ? DefaultBaseUrl : configuredBaseUrl;
        configuredModel = string.IsNullOrWhiteSpace(configuredModel) ? DefaultModel : configuredModel;
        configuredApiKey = string.IsNullOrWhiteSpace(configuredApiKey) ? DefaultApiKey : configuredApiKey;

        return new FoundryLocalSampleSettings(
            NormalizeBaseUrl(configuredBaseUrl),
            configuredBaseUrl,
            configuredModel,
            configuredApiKey);
    }

    public static async Task<FoundryLocalPreflightResult> RunPreflightAsync(string baseUrl, string configuredModel, CancellationToken cancellationToken = default)
    {
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
        HttpResponseMessage response;

        try
        {
            response = await http.GetAsync($"{baseUrl}/models", cancellationToken);
        }
        catch (Exception ex)
        {
            return FoundryLocalPreflightResult.Fail($"Could not reach Foundry Local service at '{baseUrl}': {ex.Message}");
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var snippet = string.IsNullOrWhiteSpace(body) ? "(no response body)" : body[..Math.Min(240, body.Length)];
            return FoundryLocalPreflightResult.Fail($"Foundry Local service returned {(int)response.StatusCode} {response.ReasonPhrase}. Body: {snippet}");
        }

        var payload = await response.Content.ReadAsStringAsync(cancellationToken);
        var models = ExtractModels(payload);
        if (models.Count == 0)
        {
            return FoundryLocalPreflightResult.Fail("Foundry Local responded, but no models were listed at /v1/models.");
        }

        var configuredModelEntry =
            models.FirstOrDefault(m => string.Equals(m.Id, configuredModel, StringComparison.OrdinalIgnoreCase));
        if (configuredModelEntry is not null)
        {
            return FoundryLocalPreflightResult.Success(configuredModelEntry.Id);
        }

        var fallbackModel = models.FirstOrDefault(IsLikelyChatCapableModel);
        if (fallbackModel is null)
        {
            var listedModels = string.Join(", ", models.Take(6).Select(m => m.Id));
            return FoundryLocalPreflightResult.Fail(
                $"Configured model '{configuredModel}' was not found and no chat-capable fallback model was detected in /v1/models. " +
                $"Available models include: {listedModels}");
        }

        var selectedModel =
            fallbackModel.Id;

        return FoundryLocalPreflightResult.Success(selectedModel);
    }

    public static string BuildPreflightGuidance(string reason, FoundryLocalSampleSettings settings)
    {
        return $"""
Preflight check failed.
{reason}

Try this:
  1) Verify Foundry Local CLI is installed: foundry --help
  2) Start or restart the local service: foundry service start
  3) Check status and endpoint: foundry service status
  4) Optional environment overrides (PowerShell):
     $env:{BaseUrlEnvVar}="{settings.ConfiguredBaseUrl}"
     $env:{ModelEnvVar}="{settings.Model}"
     $env:{ApiKeyEnvVar}="{settings.ApiKey}"
""";
    }

    private static string NormalizeBaseUrl(string input)
    {
        var normalized = input.Trim().TrimEnd('/');
        if (!normalized.EndsWith("/v1", StringComparison.OrdinalIgnoreCase))
        {
            normalized = $"{normalized}/v1";
        }

        return normalized;
    }

    private static List<FoundryLocalModelInfo> ExtractModels(string payload)
    {
        try
        {
            using var doc = JsonDocument.Parse(payload);
            if (!doc.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            {
                return [];
            }

            var models = new List<FoundryLocalModelInfo>();
            foreach (var item in data.EnumerateArray())
            {
                if (!item.TryGetProperty("id", out var idProp))
                {
                    continue;
                }

                var id = idProp.GetString();
                if (!string.IsNullOrWhiteSpace(id))
                {
                    models.Add(new FoundryLocalModelInfo(
                        id,
                        HasPositiveCapability(item),
                        HasChatTaskHint(item)));
                }
            }

            return models;
        }
        catch (JsonException)
        {
            return [];
        }
    }

        private static bool IsLikelyChatCapableModel(FoundryLocalModelInfo model)
        {
            if (model.HasPositiveCapability || model.HasChatTaskHint)
            {
                return true;
            }

            var id = model.Id;
            if (ContainsAny(id,
                    "whisper",
                    "embed",
                    "embedding",
                    "rerank",
                    "transcription",
                    "stt",
                    "tts",
                    "audio"))
            {
                return false;
            }

            return true;
        }

        private static bool HasPositiveCapability(JsonElement item)
        {
            if (!item.TryGetProperty("capabilities", out var capabilities) || capabilities.ValueKind != JsonValueKind.Object)
            {
                return false;
            }

            foreach (var property in capabilities.EnumerateObject())
            {
                if (property.Value.ValueKind == JsonValueKind.True &&
                    ContainsAny(property.Name, "chat", "completion", "text", "generate"))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasChatTaskHint(JsonElement item)
        {
            return
                PropertyContainsAny(item, "task", "chat", "completion", "text-generation", "generate", "instruct")
                || PropertyContainsAny(item, "tasks", "chat", "completion", "text-generation", "generate", "instruct")
                || PropertyContainsAny(item, "modalities", "chat", "text")
                || PropertyContainsAny(item, "modality", "chat", "text")
                || PropertyContainsAny(item, "type", "chat", "text", "llm");
        }

        private static bool PropertyContainsAny(JsonElement item, string propertyName, params string[] tokens)
        {
            if (!item.TryGetProperty(propertyName, out var value))
            {
                return false;
            }

            return ValueContainsAny(value, tokens);
        }

        private static bool ValueContainsAny(JsonElement value, params string[] tokens)
        {
            return value.ValueKind switch
            {
                JsonValueKind.String => ContainsAny(value.GetString(), tokens),
                JsonValueKind.Array => value.EnumerateArray().Any(v => ValueContainsAny(v, tokens)),
                JsonValueKind.Object => value.EnumerateObject().Any(p =>
                    ContainsAny(p.Name, tokens) || ValueContainsAny(p.Value, tokens)),
                _ => false
            };
        }

        private static bool ContainsAny(string? input, params string[] tokens)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            return tokens.Any(token => input.Contains(token, StringComparison.OrdinalIgnoreCase));
        }
    }

public sealed record FoundryLocalSampleSettings(
    string BaseUrl,
    string ConfiguredBaseUrl,
    string Model,
    string ApiKey);

public sealed record FoundryLocalPreflightResult(bool Ok, string? SelectedModel, string? ErrorMessage)
{
    public static FoundryLocalPreflightResult Success(string model) => new(true, model, null);
    public static FoundryLocalPreflightResult Fail(string message) => new(false, null, message);
}

internal sealed record FoundryLocalModelInfo(
    string Id,
    bool HasPositiveCapability,
    bool HasChatTaskHint);
