# AI Agents

In this lesson learn to create an AI entity that... makes decisions and executes actions without continuous human interaction? That's right, AI agents are able to perform specific tasks independently.

---

**INSERT: LESSON 3 AGENT VIDEO HERE**

AI agents allow LLMs to evolve from assistants into entities capable of taking actions on behalf of users. Agents are even able to interact with other agents to perform tasks. Some of the key attributes of an agent include a level of **autonomy** allowing the agent to initiate actions based on their programming which leads to the ability for **decision-making** based on pre-defined objectives. They are also **adaptable** in that they learn and adjust to improve performance over time.

One key thing to keep in mind when building agents is that they are focused on doing only thing. You want to narrow down their purpose as much as possible.

> 🧑‍🏫**Learn more**: Learn more about the fundamentals of AI Agents [Generative AI for Beginners: AI Agents](https://github.com/microsoft/generative-ai-for-beginners/tree/main/17-ai-agents).

## Creating an AI Agent

We'll be working with a couple of new concepts in order to build an AI agent in .NET. We'll be using a new SDK and will have to do some additional setup in Azure AI Foundry to get things started.

> 🧑‍💻**Sample code**: We'll be working from the [AgentLabs-01-Simple sample](./src/AgentLabs-01-Simple/) for this lesson.
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
- `ThreadRun`: A thread on which messages are passed to the agent on. The thread is started and can be provided additional instructions and then is polled as to its status.

Let's see a simple example of this in action!

### Build a math agent

We'll be building a single purpose agent that acts as a tutor to math students. It's sole purpose in life is to solve and then explain math problems the user asks.

1. To start with, we need to create an `AgentsClient` object that is responsible for managing the connection to Azure, the agent itself, the threads, the messages, and so on.

    ```csharp
    string projectConnectionString = "< YOU GET THIS FROM THE PROJECT IN AI FOUNDRY >";
    AgentsClient client = new(projectConnectionString, new DefaultAzureCredential());
    ```

    You can find the project connection string in AI Foundry by opening up the Hub you created, then the project. It will be on the right-hand side.

    ![Screenshot of the project homepage in AI Foundry with the project connection string highlighted in red](./images/project-connection-string.png)

1. Next we want to create the tutor agent. Remeber, it should be focused only on one thing.
   
    ```csharp
    Agent tutorAgent = (await client.CreateAgentAsync(
    model: "gpt-4o",
    name: "Math Tutor",
    instructions: "You are a personal math tutor. Write and run code to answer math questions.",
    tools: [new CodeInterpreterToolDefinition()])).Value;
    ```

    A couple of things to note here. The first is the `tools` parameter. We're creating a `CodeInterpreterToolDefinition` object (that is apart of the **Azure.AI.Projects** SDK) that will allow the agent to create and execute code.

    > 🗒️**Note**: You can create your own tools too. See the [Functions](./src/AgentLabs-02-Functions/) to learn more.

    Second note the `instructions` that are being sent along. It's a prompt and we're limiting it to answer math questions. Then last creating the agent is an async operation. That's because it's creating an object within Azure AI Foundry Agents service. So we both `await` the `CreateAgentAsync` function and then grab the `Value` of its return to get at the actual `Agent` object. You'll see this pattern occur over and over again when creating objects with the **Azure.AI.Projects** SDK.

1. An `AgentThread` is an object that handles the communication between individual agents and the user and so on. We'll need to create that so we can add a `ThreadMessage` on to it. And in this case it's the user's first question.

    ```csharp
    AgentThread thread = (await client.CreateThreadAsync()).Value;

    // Creating the first user message to AN agent - notice how we're putting it on a thread
    ThreadMessage userMessage = (await client.CreateMessageAsync(
        thread.Id,
        MessageRole.User,
        "Hello, I need to solve the equation `3x + 11 = 14`. Can you help me?")
    ).Value;
    ```

    Note the `ThreadMessage` has a type of `MessageRole.User`. And notice we're not sending the message to a specific agent, rather we're just putting it onto a thread.

1. Next up, we're going to get the agent to provide an initial response and put that on the thread and then kick the thread off. When we start the thread we're going to provide the initial agent's id to run and any additional instructions.

    ```csharp
    ThreadMessage agentMessage =  await client.CreateMessageAsync(
        thread.Id,
        MessageRole.Agent,
        "Please address the user as their name. The user has a basic account, so just share the answer to the question.")
    ).Value;

    ThreadRun run = (await client.CreateRunAsync(
        thread.Id,
        assistantId: agentMathTutor.Id, 
        additionalInstructions: "You are working in FREE TIER EXPERIENCE mode`, every user has premium account for a short period of time. Explain detailed the steps to answer the user questions")
    ).Value;
    ```

1. All that's left then is to check the status of the run

    ```csharp
    do
    {
        await Task.Delay(Timespan.FromMilliseconds(100));
        run = (await client.GetRunAsync(thread.Id, run.Id)).Value;

        Console.WriteLine($"Run Status: {run.Status}");
    }
    while (run.Status == RunStatus.Queued || run.Status == RunStatus.InProgress);
    ```

1. And then display the messages from the results

    ```csharp
    Response<PageableList<ThreadMessage>> afterRunMessagesResponse = await client.GetMessagesAsync(thread.Id);
    IReadOnlyList<ThreadMessage> messages = afterRunMessagesResponse.Value.Data;

    // sort by creation date
    messages = messages.OrderBy(m => m.CreatedAt).ToList();

    foreach (ThreadMessage msg in messages)
    {
        Console.Write($"{msg.CreatedAt:yyyy-MM-dd HH:mm:ss} - {msg.Role,10}: ");

        foreach (MessageContent contentItem in msg.ContentItems)
        {
            if (contentItem is MessageTextContent textItem)
                Console.Write(textItem.Text);
        }
        Console.WriteLine();
    }
    ```

> 🙋 **Need help?**: If you encounter any issues, [open an issue in the repository](https://github.com/microsoft/Generative-AI-for-beginners-dotnet/issues/new).

The logical next step is to start to use multiple agents to create an automous system. A next step might be to have an agent that checks to see if the user has a premium account or not.

## Summary

AI Agents are autonomous AI entities that go beyond simple chat interactions - they can:

- Make Independent Decisions: Execute tasks without constant human input
- Maintain Context: Hold state and remember previous interactions
- Use Tools: Access external systems and APIs to accomplish tasks
- Collaborate: Work with other agents to solve complex problems

And you learned how to use the **Azure AI Agents** service with the **Azure AI Project** SDK to create a rudimentary agent.

Think of agents as AI assistants with agency - they don't just respond, they act based on their programming and objectives.

## Additional resources

- [Build a minimal agent with .NET](https://learn.microsoft.com/dotnet/ai/quickstarts/quickstart-assistants?pivots=openai)
- [Multi-agent orchestration](https://techcommunity.microsoft.com/blog/educatordeveloperblog/using-azure-ai-agent-service-with-autogen--semantic-kernel-to-build-a-multi-agen/4363121)
- [Semantic Kernel Agent Framework](https://learn.microsoft.com/semantic-kernel/frameworks/agent/?pivots=programming-language-csharp)
- [AI Agents - Beginners Series to GenAI](https://github.com/microsoft/generative-ai-for-beginners/tree/main/17-ai-agents)

## Next Steps

You've come a long way! From learning about simple one and done text completions to building agents!

👉 [In the next lesson see some real-life practical examples](../04-PracticalSamples/readme.md) of using everything together.