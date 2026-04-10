using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry;

namespace MAF_MultiAgents;

/// <summary>
/// Provides factory methods for creating and managing AI Foundry agents using the Responses API.
/// </summary>
class AIFoundryAgentsProvider
{
    private static readonly AppConfigurationService _config = AppConfigurationService.Instance;

    /// <summary>
    /// Creates an AI agent in Microsoft Foundry using the Responses API.
    /// </summary>
    /// <param name="name">The name of the agent.</param>
    /// <param name="instructions">The instructions that define the agent's behavior.</param>
    /// <returns>A configured AIAgent instance.</returns>
    public static AIAgent CreateAIAgent(string name, string instructions)
    {
        var projectClient = CreateProjectClient();

        AIAgent aiAgent = projectClient.AsAIAgent(
            model: _config.DeploymentName,
            instructions: instructions,
            name: name);

        return aiAgent;
    }

    /// <summary>
    /// Deletes an existing AI agent from Microsoft Foundry.
    /// Responses API agents are ephemeral and do not persist on the server,
    /// so this method is a no-op.
    /// </summary>
    /// <param name="agent">The agent to delete.</param>
    public static void DeleteAIAgentInAIFoundry(AIAgent agent)
    {
        // Responses API agents are ephemeral — no server-side cleanup needed.
    }

    /// <summary>
    /// Creates an AIProjectClient using Azure CLI credentials.
    /// </summary>
    private static AIProjectClient CreateProjectClient()
    {
        return new AIProjectClient(
            new Uri(_config.AzureFoundryProjectEndpoint!),
            new AzureCliCredential());
    }
}
