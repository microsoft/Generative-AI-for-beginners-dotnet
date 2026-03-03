using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Bot.ObjectModel;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.ComponentModel;

// =============================================================================
// STEP 1: Create a shared chat client (Ollama running locally)
// =============================================================================
IChatClient chatClient = new OllamaApiClient(
    new Uri("http://localhost:11434/"),
    "ministral-3");

// =============================================================================
// STEP 2: Define tools that agents can use
// =============================================================================
AIFunction[] tools = [AIFunctionFactory.Create(GetWeather, "GetWeather")];

// =============================================================================
// STEP 3: Create an agent factory
// =============================================================================
var agentFactory = new ChatClientPromptAgentFactory(chatClient, tools);

// =============================================================================
// STEP 4: Define each agent with its role and instructions
// =============================================================================

// Triage Agent - Routes requests to the appropriate specialist
var triageAgent = await agentFactory.CreateAsync(promptAgent: new GptComponentMetadata(
    name: "triage_agent",
    instructions: ToInstructions("""
        You are a triage agent for a mini concierge.

        Decide who should own the task, then HANDOFF to exactly ONE agent:
        - travel_agent: travel planning, destinations, weather
        - vision_agent: anything that requires looking at an image
        - general_agent: everything else

        Do not answer the user yourself. Always handoff.
        """)));

// Travel Agent - Handles travel and weather questions
var travelAgent = await agentFactory.CreateAsync(promptAgent: new GptComponentMetadata(
    name: "travel_agent",
    instructions: ToInstructions("""
        You are a travel concierge.
        If the user asks about weather, call the GetWeather tool.
        Keep answers short and actionable.
        """)));

// Vision Agent - Analyzes images
var visionAgent = await agentFactory.CreateAsync(promptAgent: new GptComponentMetadata(
    name: "vision_agent",
    instructions: ToInstructions("""
        You are a visual inspector.
        When given an image, describe what you see in 3-5 bullet points.
        Avoid guessing brands or private details.
        """)));

// General Agent - Handles everything else
var generalAgent = await agentFactory.CreateAsync(promptAgent: new GptComponentMetadata(
    name: "general_agent",
    instructions: ToInstructions("""
        You are a helpful general assistant.
        Keep answers concise.
        """)));

// =============================================================================
// STEP 5: Build the handoff workflow
// =============================================================================
// The triage agent can hand off to any specialist.
// Each specialist can hand back to triage when done.
Func<Workflow> workflowFactory = () =>
    AgentWorkflowBuilder.CreateHandoffBuilderWith(triageAgent)
        .WithHandoffs(triageAgent, [travelAgent, visionAgent, generalAgent])
        .WithHandoff(travelAgent, triageAgent)
        .WithHandoff(visionAgent, triageAgent)
        .WithHandoff(generalAgent, triageAgent)
        .Build();

// =============================================================================
// STEP 6: Run demos
// =============================================================================

// Demo 1: General Q&A → triage hands off to the general agent
Console.WriteLine("---- DEMO #1: General Q&A ----");
await RunWorkflowAsync(
    workflowFactory: workflowFactory,
    messages: [new ChatMessage(ChatRole.User, "What is the capital of France?")]);

// Demo 2: Weather question → triage hands off to the travel agent (with tool call)
Console.WriteLine("---- DEMO #2: Weather (Tool Calling) ----");
await RunWorkflowAsync(
    workflowFactory: workflowFactory,
    messages: [new ChatMessage(ChatRole.User, "I'm going to Amsterdam tomorrow. What's the weather in celsius?")]);

// Demo 3: Image question → triage hands off to the vision agent (multimodal)
Console.WriteLine("---- DEMO #3: Image Analysis (Multimodal) ----");
byte[] imageBytes = File.ReadAllBytes("image01.jpg");
ChatMessage imageMessage = new(ChatRole.User, [
    new TextContent("What do you see in this image?"),
    new DataContent(imageBytes, "image/jpeg")
]);
await RunWorkflowAsync(
    workflowFactory: workflowFactory,
    messages: [imageMessage]);

// =============================================================================
// Helper Methods
// =============================================================================

static TemplateLine ToInstructions(string text) =>
    new([TemplateSegment.FromText(text)]);

static async Task RunWorkflowAsync(Func<Workflow> workflowFactory, List<ChatMessage> messages)
{
    Workflow workflow = workflowFactory();
    StreamingRun run = await InProcessExecution.StreamAsync(workflow, messages);
    await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

    string? currentAgent = null;

    await foreach (WorkflowEvent evt in run.WatchStreamAsync())
    {
        switch (evt)
        {
            case AgentRunUpdateEvent agentUpdate:
                if (currentAgent != agentUpdate.ExecutorId)
                {
                    currentAgent = agentUpdate.ExecutorId;
                    Console.WriteLine($"[AGENT: {currentAgent}]");
                }
                break;

            case WorkflowOutputEvent output:
                var outputMessages = (List<ChatMessage>)output.Data!;
                Console.WriteLine($"Final Answer: {outputMessages.LastOrDefault()?.Text}");
                Console.WriteLine();
                return;
        }
    }
}

// =============================================================================
// Tool Definition
// =============================================================================

[Description("Get the weather for a given location.")]
static string GetWeather(
    [Description("The city and state, e.g. San Francisco, CA")] string location,
    [Description("The unit of temperature: 'celsius' or 'fahrenheit'")] string unit)
{
    string temperature = unit.Equals("celsius", StringComparison.OrdinalIgnoreCase) ? "15°C" : "59°F";
    return $"The weather in {location} is cloudy with a high of {temperature}.";
}
