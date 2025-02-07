# AI Agents

In this lesson learn to create an AI entity that... makes decisions and executes actions without continuous human interaction? That's right, AI agents are able to perform specific tasks independently.

---

**INSERT: LESSON 3 AGENT VIDEO HERE**

AI agents allow LLMs to evolve from assistants into entities capable of taking actions on behalf of users. Agents are even able to interact with other agents to perform tasks. Some of the key attributes of an agent include a level of **autonomy** allowing the agent to initiate actions based on their programming which leads to the ability for **decision-making** based on pre-defined objectives. They are also **adaptable** in that they learn and adjust to improve performance over time.

> 🧑‍🏫**Learn more**: Learn more about the fundamentals of AI Agents [Generative AI for Beginners: AI Agents](https://github.com/microsoft/generative-ai-for-beginners/tree/main/17-ai-agents).

## Creating an AI Agent

We'll be working with a couple of new concepts in order to build an AI agent in .NET. We'll have to do some additional setup in Azure AI Foundry to get things started.

> 🧑‍💻**Code sample:** We'll be working from the [AgentLabs-01-Simple sample](./src/AgentLabs-01-Simple/) for this lesson.
>
> We did include some more advanced samples in the `/src/` folder as well. You can view the README's of [AgentLabs-02-Functions](./src/AgentLabs-02-Functions/) or [AgentLabs-03-OpenAPIs](./src/AgentLabs-03-OpenAPIs/) or [AgentLabs-03-PythonParksInformationServer](./src/AgentLabs-03-PythonParksInformationServer/) for more info on them.

### Azure AI Agent Service

We're going to introduce a new Azure Service that will help us build agents, the appropriately named [Azure AI Agent Service](https://learn.microsoft.com/azure/ai-services/agents/overview).

To run the code samples included in this lesson, you'll need to perform some additional setup in Azure AI Foundry. You can follow [these instructions to setup a **Basic Agent**](https://learn.microsoft.com/azure/ai-services/agents/quickstart?pivots=programming-language-csharp).

### Azure AI Projects library

Agents are composed of 3 parts. The **LLM** or the model. **State** or the context (much like a conversation) that helps guide decisions based off of past results. And **Tools** which are like [functions we learned about before](./01-lm-completions-functions.md#function-calling) that allow a bridge between the model and external systems.

So, in theory, you could build AI Agents with what you've learned already. But the **Azure AI Projects for .NET** library makes developing agents easier by providing an API that streamlines a lot of the typical tasks for you.

There are a couple of concepts (which map to classes) to understand when working with the Azure AI Projects library.

- `AgentClient`: The overall client that creates and hosts the agents, manages threads in which they run, and handles the connection to the cloud.
- `Agent`: The agent that holds instructions on what it's to do as well as definitions for tools it has access to.
- `ThreadMessage`: These are messages - almost like prompts we learned about before - that get passed to the agent. Agents also create `ThreadMessage` objects to communicate.
- `AgentThread`: A thread on which messages are passed to the agent on. The thread is started and can be provided additional instructions and then is polled as to its status.

Let's see a simple example of this in action!

### Build a math agent





```csharp
// Configure the connection to the Cloud, AI models and be the base for the agent
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var options = new DefaultAzureCredentialOptions
{
    ExcludeEnvironmentCredential = true,
    ExcludeWorkloadIdentityCredential = true,
    TenantId = config["tenantid"]
};
var connectionString = config["connectionString"];

// Create the agent client with the connection string and the Azure credentials
AgentsClient client = new AgentsClient(connectionString, new AgentsClient client = new AgentsClient(connectionString, new DefaultAzureCredential(options));
```

For the agent, we can see how to create a simple agent that gets helps in simple math, look into the Agent code:

```csharp
// Create Agent with the model, name, instructions and tools, to do that we use the Agent Client
Response<Agent> agentResponse = await client.CreateAgentAsync(
    model: "gpt-4o-mini",
    name: "Math Tutor",
    instructions: "You are a personal math tutor. Write and run code to answer math questions.",
    tools: [new CodeInterpreterToolDefinition()]);
Agent agentMathTutor = agentResponse.Value;
// Use the Agent to create a thread and start the conversation with the Client
Response<AgentThread> threadResponse = await client.CreateThreadAsync();
AgentThread thread = threadResponse.Value;

```	

Look how the Agent Client and the Agent are interconnected, the Agent Client is the base for the Agent, and the Agent is the one that performs the tasks.

For the tools, we can see how to create a tool for a travel agency, look into the `src/Agents-02-TravelAgency` Agent:

```csharp
// create Agent
Response<Agent> agentResponse = await client.CreateAgentAsync(
    model: "gpt-4o-mini",
    name: "SDK Test Agent - Vacation",
        instructions: @"You are a travel assistant. Use the provided functions to help answer questions. 
Customize your responses to the user's preferences as much as possible. Write and run code to answer user questions.",
    // Add the tools to the agent, tools are the plugins and functions that the agent uses to perform the tasks. 
    tools: new List<ToolDefinition> {        
        CityInfo.getUserFavoriteCityTool,
        CityInfo.getWeatherAtLocationTool,
        CityInfo.getParksAtLocationTool}
    );
Agent agentTravelAssistant = agentResponse.Value;
Response<AgentThread> threadResponse = await client.CreateThreadAsync();
AgentThread thread = threadResponse.Value;
```

The tools are the plugins and functions that the agent uses to perform the tasks. In this case, the agent uses the CityInfo tools to get information about cities. Know more about `CityInfo` tools in the `src/Agents-02-TravelAgency` sample.


## Conclusions and resources

In this chapter, we explored the core generative AI techniques, including Language Model Completions, RAG, Vision and Audio applications. 

In the next chapter, we will explore how to implement these techniques in practice, using real-world examples and complex samples.

### Additional Resources

> ⚠️ **Note**: If you encounter any issues, open an issue in the repository.

- [GitHub Codespaces Documentation](https://docs.github.com/en/codespaces)
- [GitHub Models Documentation](https://docs.github.com/en/github-models/prototyping-with-ai-models)
- [Generative AI for Beginners](https://github.com/microsoft/generative-ai-for-beginners)
- [Semantic Kernel Documentation](https://learn.microsoft.com/en-us/semantic-kernel/get-started/quick-start-guide?pivots=programming-language-csharp)
- [MEAI Documentation](https://devblogs.microsoft.com/dotnet/introducing-microsoft-extensions-ai-preview/)

### Next Steps

Next, we'll explore some samples in how to implement these algoritms pratically. 

<p align="center">
    <a href="../04-Practical.NETGenAISamples/readme.md">Go to Chapter 4</a>
</p>