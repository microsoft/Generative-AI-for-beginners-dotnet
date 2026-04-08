using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MAF_HostedAgent_02_MultiAgent.Agents;

/// <summary>
/// The Researcher agent takes a topic and generates key questions,
/// facts, and research points for further writing.
/// </summary>
static class ResearcherAgent
{
    public static AIAgent Create(IChatClient client) =>
        client.AsAIAgent(
            name: "Researcher",
            instructions: """
                You are a thorough research assistant. When given a topic, produce a well-organized
                research brief that includes:
                - A one-sentence summary of the topic
                - 3-5 key questions worth exploring
                - 5-8 important facts or talking points
                - Notable experts or sources to reference

                Keep the output concise and factual. Use bullet points for clarity.
                Your research will be handed to a writer who will craft an article from it.
                """);
}
