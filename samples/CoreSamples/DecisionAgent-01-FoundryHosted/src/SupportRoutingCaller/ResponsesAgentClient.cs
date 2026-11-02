/*
 * Purpose: Call a deployed Responses endpoint and return only a completed typed decision.
 * 1. Request a Foundry token for the caller's Azure CLI identity.
 * 2. Verify both HTTP success and the final Responses status.
 * 3. Deserialize the agent's JSON into the shared public contract.
 */
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Azure.Core;
using SupportRouting;

namespace SupportRoutingCaller;

public sealed class ResponsesAgentClient
{
    private const string FoundryScope = "https://ai.azure.com/.default";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly TokenCredential _credential;
    private readonly Uri _responsesEndpoint;
    private readonly string _agentName;

    public ResponsesAgentClient(
        HttpClient httpClient,
        TokenCredential credential,
        Uri agentOpenAiBaseUrl,
        string agentName)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(credential);
        ArgumentNullException.ThrowIfNull(agentOpenAiBaseUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(agentName);

        if (!agentOpenAiBaseUrl.IsAbsoluteUri ||
            agentOpenAiBaseUrl.Scheme != Uri.UriSchemeHttps)
        {
            throw new ArgumentException("The agent OpenAI-compatible base URL must be an absolute HTTPS URI.", nameof(agentOpenAiBaseUrl));
        }

        _httpClient = httpClient;
        _credential = credential;
        _responsesEndpoint = new Uri($"{agentOpenAiBaseUrl.AbsoluteUri.TrimEnd('/')}/responses");
        _agentName = agentName;
    }

    public async Task<RoutingDecision> InvokeAsync(
        string ticket,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(ticket);

        var accessToken = await _credential.GetTokenAsync(
            new TokenRequestContext([FoundryScope]),
            cancellationToken);

        using var request = new HttpRequestMessage(HttpMethod.Post, _responsesEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);
        request.Content = JsonContent.Create(new
        {
            model = _agentName,
            input = ticket,
            stream = false,
            store = false
        });

        using var response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        using var document = await JsonDocument.ParseAsync(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            cancellationToken: cancellationToken);

        var root = document.RootElement;
        var status = root.TryGetProperty("status", out var statusElement)
            ? statusElement.GetString()
            : null;

        if (!string.Equals(status, "completed", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"The Foundry Responses request did not complete successfully. Final status: {status ?? "missing"}.");
        }

        var decisionJson = FindOutputText(root);
        return JsonSerializer.Deserialize<RoutingDecision>(decisionJson, JsonOptions)
            ?? throw new InvalidDataException("The completed Foundry response did not contain a routing decision.");
    }

    private static string FindOutputText(JsonElement response)
    {
        if (response.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in output.EnumerateArray())
            {
                if (!item.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
                {
                    continue;
                }

                foreach (var part in content.EnumerateArray())
                {
                    if (part.TryGetProperty("type", out var type) &&
                        type.GetString() == "output_text" &&
                        part.TryGetProperty("text", out var text) &&
                        text.ValueKind == JsonValueKind.String)
                    {
                        return text.GetString()
                            ?? throw new InvalidDataException("The completed Foundry response contained null output text.");
                    }
                }
            }
        }

        throw new InvalidDataException("The completed Foundry response did not contain output text.");
    }
}
