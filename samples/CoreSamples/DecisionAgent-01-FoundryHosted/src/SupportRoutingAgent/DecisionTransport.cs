/*
 * Purpose: Keep native decision transport telemetry free of request content.
 * Only the HTTP status is recorded; authentication and payload handling stay in the SDK.
 */
namespace SupportRouting;

public sealed class DecisionTransport(ILogger logger) : DelegatingHandler(
    new SocketsHttpHandler { AllowAutoRedirect = false })
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        logger.LogInformation("Native decision HTTP {StatusCode}", (int)response.StatusCode);
        return response;
    }
}
