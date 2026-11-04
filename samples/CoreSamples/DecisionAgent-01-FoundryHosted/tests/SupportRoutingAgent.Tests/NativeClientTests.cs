/*
 * Purpose: Verify the community client's native request contract with synthetic HTTP.
 * Fake credentials prove the expected audience without obtaining or storing a real token.
 */
using System.Net;
using System.Text;
using System.Text.Json;
using Azure.Core;
using ElBruno.AI.Decisions;
using ElBruno.AI.Decisions.Foundry;
using SupportRouting;
using Xunit;

namespace SupportRoutingTests;

public sealed class NativeClientTests
{
    [Fact]
    public async Task CallsNativeSystemOneWithExplicitCredential()
    {
        var credential = new FixtureCredential();
        using var handler = new FixtureHandler(
            HttpStatusCode.OK,
            """{"answers":{"decision":{"type":"choice","choice":"billing","confidence":0.61,"probabilities":{"billing":0.7,"technical-support":0.1,"account-access":0.1,"manual-review":0.1}}},"usage":{"input_tokens":12,"output_tokens":0}}""");
        using var http = new HttpClient(handler);
        using var client = Create(http, credential);

        var result = await client.ChooseAsync("synthetic", "Which team?", SupportRoutingAgent.Routes);

        Assert.Equal(
            "https://example.services.ai.azure.com/providers/microsoft/v1/systemone",
            handler.Url);
        Assert.Equal("Bearer", handler.AuthorizationScheme);
        Assert.False(handler.HasApiKey);
        Assert.Equal("https://cognitiveservices.azure.com/.default", credential.Scope);
        using var body = JsonDocument.Parse(handler.Body!);
        Assert.Equal("test-decision", body.RootElement.GetProperty("model").GetString());
        Assert.Equal("synthetic", body.RootElement.GetProperty("state").GetString());
        Assert.Equal(
            "choice",
            body.RootElement.GetProperty("questions").GetProperty("decision").GetProperty("type").GetString());
        Assert.Equal(.61, result.Confidence);
        Assert.Equal(0, result.Usage!.OutputTokens);
        Assert.Null(result.Usage.TotalTokens);
    }

    [Theory]
    [InlineData(401)]
    [InlineData(403)]
    [InlineData(429)]
    [InlineData(500)]
    public async Task HttpFailureStaysFailure(int status)
    {
        using var handler = new FixtureHandler(
            (HttpStatusCode)status,
            """{"error":{"code":"SyntheticFailure"}}""");
        using var http = new HttpClient(handler);
        using var client = Create(http, new FixtureCredential());

        var error = await Assert.ThrowsAsync<DecisionException>(() =>
            client.ChooseAsync("synthetic", "Which team?", SupportRoutingAgent.Routes));

        Assert.Equal(status, error.StatusCode);
    }

    [Fact]
    public async Task MalformedResultStaysFailure()
    {
        using var handler = new FixtureHandler(HttpStatusCode.OK, """{"answers":{}}""");
        using var http = new HttpClient(handler);
        using var client = Create(http, new FixtureCredential());

        await Assert.ThrowsAsync<DecisionException>(() =>
            client.ChooseAsync("synthetic", "Which team?", SupportRoutingAgent.Routes));
    }

    private static FoundryDecisionClient Create(HttpClient http, TokenCredential credential) =>
        new(http, new FoundryDecisionOptions
        {
            Endpoint = new Uri("https://example.services.ai.azure.com/"),
            Model = "test-decision",
            Credential = credential
        });

    private sealed class FixtureCredential : TokenCredential
    {
        public string? Scope { get; private set; }

        public override AccessToken GetToken(
            TokenRequestContext requestContext,
            CancellationToken cancellationToken)
        {
            Scope = Assert.Single(requestContext.Scopes);
            return new AccessToken("synthetic-not-a-real-token", DateTimeOffset.UtcNow.AddMinutes(1));
        }

        public override ValueTask<AccessToken> GetTokenAsync(
            TokenRequestContext requestContext,
            CancellationToken cancellationToken) =>
            new(GetToken(requestContext, cancellationToken));
    }

    private sealed class FixtureHandler(HttpStatusCode status, string content) : HttpMessageHandler
    {
        public string? Url { get; private set; }
        public string? AuthorizationScheme { get; private set; }
        public bool HasApiKey { get; private set; }
        public string? Body { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Url = request.RequestUri!.AbsoluteUri;
            AuthorizationScheme = request.Headers.Authorization?.Scheme;
            HasApiKey = request.Headers.Contains("api-key");
            Body = await request.Content!.ReadAsStringAsync(cancellationToken);

            return new HttpResponseMessage(status)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }
    }
}
