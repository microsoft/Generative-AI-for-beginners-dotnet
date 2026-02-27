using Azure.AI.Agents.Persistent;
using Azure.Identity;
using Microsoft.Agents.AI;

namespace MAF_MultiAgents;

/// <summary>
/// Provides factory methods for creating and managing AI Foundry persistent agents.
/// </summary>
class AIFoundryAgentsProvider
{
    private static readonly AppConfigurationService _config = AppConfigurationService.Instance;

    /// <summary>
    /// Creates a persistent AI agent in Microsoft Foundry.
    /// </summary>
    /// <param name="name">The name of the agent.</param>
    /// <param name="instructions">The instructions that define the agent's behavior.</param>
    /// <returns>A configured AIAgent instance.</returns>
    public static AIAgent CreateAIAgent(string name, string instructions)
    {
        var persistentAgentsClient = CreatePersistentAgentsClient();

        AIAgent aiAgent = persistentAgentsClient.CreateAIAgent(
            model: _config.DeploymentName,
            name: name,
            instructions: instructions);

        return aiAgent;
    }

    /// <summary>
    /// Deletes an existing AI agent from Microsoft Foundry.
    /// </summary>
    /// <param name="agent">The agent to delete.</param>
    public static void DeleteAIAgentInAIFoundry(AIAgent agent)
    {
        var persistentAgentsClient = CreatePersistentAgentsClient();
        persistentAgentsClient.Administration.DeleteAgent(agent.Id);
    }

    /// <summary>
    /// Creates a PersistentAgentsClient using Azure CLI credentials.
    /// </summary>
    private static PersistentAgentsClient CreatePersistentAgentsClient()
    {
        return new PersistentAgentsClient(
            _config.AzureFoundryProjectEndpoint!,
            new AzureCliCredential());
    }
}
