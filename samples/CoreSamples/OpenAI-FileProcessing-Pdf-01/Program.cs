// based on the original sample for Semantic Kernel and Gemini
// https://medium.com/@johnidouglasmarangon/generating-structured-outputs-from-pdfs-with-semantic-kernel-and-gemini-aa4d4382e339

using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.ComponentModel;
using System.Text.Json;

// load OpenAI api_key value
var configBuilder = new ConfigurationBuilder().AddUserSecrets<Program>();
var configuration = configBuilder.Build();
string modelId = "gpt-4.1";
string apiKey = configuration["api_key"];

var builder = Kernel.CreateBuilder();
builder.AddOpenAIChatCompletion(modelId, apiKey);
var kernel = builder.Build();
var chatService = kernel.GetRequiredService<IChatCompletionService>();

var filePath = Path.Combine(Directory.GetCurrentDirectory(), "docs", "real-state-contract-1.pdf");
var bytes = File.ReadAllBytes(filePath);

var history = new ChatHistory();
history.AddSystemMessage(
    """
    You are an expert real estate contract analyst. 
    Your task is to extract key information from the provided real estate purchase agreement. 
    Pay close attention to detail and extract the following data points. 
    If a data point is not explicitly mentioned in the document, mark it as "N/A".
    Format your response as a JSON object, where the keys are the data point names and the values are the extracted information. 
    Do not include any explanatory text or comments in your response, just the raw JSON.
    The real estate purchase agreement is provided below. 
    Process it carefully and provide the JSON output.
    """
);

#pragma warning disable SKEXP0001 
history.AddUserMessage([
    new TextContent("This is the PDF file"),
    new BinaryContent(bytes, "application/pdf")
]);

var executionSettings = new OpenAIPromptExecutionSettings
{
    MaxTokens = null
};

executionSettings.ResponseFormat = typeof(Contract);


var response = await chatService.GetChatMessageContentAsync(history, executionSettings);
Console.WriteLine("Response received from OpenAI API:");
Console.WriteLine(response.Content);
Console.WriteLine("---");

var contract = JsonSerializer.Deserialize<Contract>(response.ToString());
Console.WriteLine("Extracted Contract Information:");
Console.WriteLine(JsonSerializer.Serialize(contract, new JsonSerializerOptions { WriteIndented = true }));

public class Seller
{
    [Description("The full legal name of the seller")]
    public string Name { get; set; }

    [Description("The full address of the seller")]
    public string Address { get; set; }
}

public class Buyer
{
    [Description("The full legal name of the buyer")]
    public string Name { get; set; }

    [Description("The full address of the buyer")]
    public string Address { get; set; }
}

public class Contract
{
    [Description("The full street address of the property")]
    public string PropertyAddress { get; set; }

    [Description("The total purchase price of the property")]
    public string PropertyPrice { get; set; }

    [Description("The full legal description of the property")]
    public string PropertyDescription { get; set; }

    [Description("The date the agreement was signed")]
    public string Date { get; set; }

    public Seller Seller { get; set; }
    public Buyer Buyer { get; set; }
}