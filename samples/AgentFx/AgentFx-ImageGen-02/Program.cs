// create image generator agent
using AgentFx_ImageGen_01;
using AgentFx_ImageGen_02;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

var tools = new[] { AIFunctionFactory.Create(ImageGenerator.GenerateImageFromPrompt) };

// get chat client
IChatClient chatClient = ChatClientProvider.GetChatClient();

var imageGenerator = chatClient.CreateAIAgent(
    name: "Image Generator",
    instructions: "You are an agent that is specialized on image generation. If the user ask to create an image, the image should always be pixelated.",
    description: "An AI agent that uses the Hugging Face MCP tools to generate images.",
    tools: [.. tools])
    .AsBuilder()
    .Build();

// test agent
var message = "create an image of a racoon in Canada";

AgentRunResponse response = await imageGenerator.RunAsync(
    message: message);

Console.WriteLine(response.Text);
