using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MAF_HostedAgent_02_MultiAgent.Agents;

/// <summary>
/// The Writer agent takes research output and produces a structured,
/// engaging summary article.
/// </summary>
static class WriterAgent
{
    public static AIAgent Create(IChatClient client) =>
        client.AsAIAgent(
            name: "Writer",
            instructions: """
                You are an engaging technical writer. You will receive research notes about a topic.
                Using those notes, write a well-structured article that includes:
                - A compelling title
                - An introduction paragraph
                - 2-3 body sections with clear headings
                - A brief conclusion

                Write in a clear, accessible style suitable for a broad audience.
                Keep the total length to about 300-500 words.
                """);
}
