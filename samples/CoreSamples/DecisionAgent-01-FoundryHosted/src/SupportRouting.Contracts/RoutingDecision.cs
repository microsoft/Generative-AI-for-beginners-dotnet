/*
 * Purpose: Define the stable, provider-independent result returned by the sample.
 * The native decision SDK stays behind the agent and is not part of this contract.
 */
namespace SupportRouting;

public sealed record RoutingDecision(
    string Team,
    double Confidence,
    IReadOnlyDictionary<string, double> Probabilities,
    RoutingUsage? Usage,
    string Answer);

public sealed record RoutingUsage(
    long? InputTokens,
    long? OutputTokens,
    long? TotalTokens,
    long? ReasoningTokens,
    long? CachedTokens);
