#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0003
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0011
#pragma warning disable SKEXP0050
#pragma warning disable SKEXP0052

using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;

// system message
var systemMessage = "You are a helpful assistant. You reply in short and precise answers, and you explain your responses. If you don't know an answer, you reply 'I don't know'";

// questions
var question = "What is Bruno's favourite super hero?";

// intro
Console.WriteLine($"Question: {question}");

var modelId = "llama3.2";

// Create a chat completion service
var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion(
    modelId: modelId,
    endpoint: new Uri("http://localhost:11434/v1"),
    apiKey: "apikey");
builder.AddLocalTextEmbeddingGeneration();
Kernel kernel = builder.Build();
var chat = kernel.GetRequiredService<IChatCompletionService>();

// read the file "text.txt" from disk and store each line in a array of strings


// no memory
Console.WriteLine($"{modelId} response (no memory).");
var history = new ChatHistory();
history.AddSystemMessage(systemMessage);

history.AddUserMessage(question);
var response = chat.GetStreamingChatMessageContentsAsync(history);
await foreach (var result in response)
{
    Console.WriteLine(result.ToString());
}

// separator
Console.WriteLine("");
Console.WriteLine("Press Enter to continue");
Console.ReadLine();
Console.WriteLine($"{modelId} response (using semantic memory).");

// Using memory
history = new ChatHistory();
history.AddSystemMessage(systemMessage);

// get the embeddings generator service
var embeddingGenerator = kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();
var memory = new SemanticTextMemory(new VolatileMemoryStore(), embeddingGenerator);

// add facts to the collection
Dictionary<string, string> memoryInformation = new()
{
    {"1", "Gisela's favourite super hero is Batman" },
    {"2", "The last super hero movie watched by Gisela was Guardians of the Galaxy Vol 3" },
    {"3", "Bruno's favourite super hero is Invincible" },
    {"4", "The last super hero movie watched by Bruno was Deadpool and Wolverine" },
    {"5", "Bruno does not like the super hero movie: Eternals" }
};

const string MemoryCollectionName = "fanFacts";
foreach (var information in memoryInformation)
{
    await memory.SaveInformationAsync(MemoryCollectionName,
        id: information.Key,
        text: information.Value);
}

TextMemoryPlugin memoryPlugin = new(memory);

// Import the text memory plugin into the Kernel.
kernel.ImportPluginFromObject(memoryPlugin);

OpenAIPromptExecutionSettings settings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
};

var prompt = @"Question: {{$input}}
    Answer the question using the memory content: {{Recall}}";

history.AddUserMessage(prompt);

var arguments = new KernelArguments(settings)
{
    { "input", question },
    { "collection", MemoryCollectionName },
    { "messages", history }
};

var newResponse = kernel.InvokePromptStreamingAsync<StreamingChatMessageContent>(prompt, arguments);
await foreach (var result in newResponse)
{
    Console.WriteLine(result.ToString());
}

Console.WriteLine($"");