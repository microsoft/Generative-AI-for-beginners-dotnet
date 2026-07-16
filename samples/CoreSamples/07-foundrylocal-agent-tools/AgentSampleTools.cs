using Microsoft.Extensions.AI;

internal static class AgentSampleTools
{
    private static int _invocationCount;

    internal static void ResetInvocationCounter()
        => Interlocked.Exchange(ref _invocationCount, 0);

    internal static int GetInvocationCount()
        => Interlocked.CompareExchange(ref _invocationCount, 0, 0);

    internal static IList<AITool> BuildTools()
    {
        return
        [
            AIFunctionFactory.Create(
                (string timezoneId) => GetTimeInTimezone(timezoneId),
                "get_time_in_timezone",
                "Get current local time for an IANA/Windows timezone id. Use 'local' for machine local time."),

            AIFunctionFactory.Create(
                (double amount, double percentage) => CalculateTip(amount, percentage),
                "calculate_tip",
                "Calculate a tip amount from bill amount and percentage."),

            AIFunctionFactory.Create(
                (string topic) => GetDemoFact(topic),
                "get_demo_fact",
                "Return a short deterministic demo fact about a topic.")
        ];
    }

    private static string GetTimeInTimezone(string timezoneId)
    {
        Interlocked.Increment(ref _invocationCount);
        Console.WriteLine($"[tool:get_time_in_timezone] called with timezoneId='{timezoneId}'");

        if (string.IsNullOrWhiteSpace(timezoneId) ||
            timezoneId.Equals("local", StringComparison.OrdinalIgnoreCase))
        {
            var result = DateTimeOffset.Now.ToString("O");
            Console.WriteLine($"[tool:get_time_in_timezone] result='{result}'");
            return result;
        }

        try
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            var result = TimeZoneInfo.ConvertTime(DateTimeOffset.Now, tz).ToString("O");
            Console.WriteLine($"[tool:get_time_in_timezone] result='{result}'");
            return result;
        }
        catch
        {
            var result = DateTimeOffset.Now.ToString("O");
            Console.WriteLine($"[tool:get_time_in_timezone] invalid timezone, fallback result='{result}'");
            return result;
        }
    }

    private static string CalculateTip(double amount, double percentage)
    {
        Interlocked.Increment(ref _invocationCount);
        Console.WriteLine($"[tool:calculate_tip] called with amount={amount}, percentage={percentage}");
        var tip = Math.Round(amount * percentage / 100.0, 2, MidpointRounding.AwayFromZero);
        var total = Math.Round(amount + tip, 2, MidpointRounding.AwayFromZero);
        var result = $"tip={tip:F2};total={total:F2}";
        Console.WriteLine($"[tool:calculate_tip] result='{result}'");
        return result;
    }

    private static string GetDemoFact(string topic)
    {
        Interlocked.Increment(ref _invocationCount);
        Console.WriteLine($"[tool:get_demo_fact] called with topic='{topic}'");

        if (string.IsNullOrWhiteSpace(topic))
        {
            const string fallback = "Local AI keeps model execution on your machine for privacy and low latency.";
            Console.WriteLine($"[tool:get_demo_fact] result='{fallback}'");
            return fallback;
        }

        var result = $"Demo fact about {topic.Trim()}: local agent workflows can use tools without a REST endpoint.";
        Console.WriteLine($"[tool:get_demo_fact] result='{result}'");
        return result;
    }
}
