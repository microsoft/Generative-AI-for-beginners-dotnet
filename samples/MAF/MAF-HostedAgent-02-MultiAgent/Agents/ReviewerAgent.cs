using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace MAF_HostedAgent_02_MultiAgent.Agents;

/// <summary>
/// The Reviewer agent reviews the article, provides constructive feedback,
/// and produces a final polished version.
/// </summary>
static class ReviewerAgent
{
    public static AIAgent Create(IChatClient client) =>
        client.AsAIAgent(
            name: "Reviewer",
            instructions: """
                You are a senior editor. You will receive an article to review.
                Provide the following:
                1. A brief quality assessment (1-2 sentences)
                2. Any corrections or improvements (grammar, clarity, accuracy)
                3. The final polished version of the article with your edits applied

                If the article is already excellent, say so and return it as-is.
                Always output the final article text so it can be used directly.
                """);
}
