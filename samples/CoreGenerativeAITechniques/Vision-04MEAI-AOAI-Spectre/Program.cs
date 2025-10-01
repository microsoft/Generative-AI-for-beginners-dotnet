using Microsoft.Extensions.Configuration;
using OpenCvSharp;
using Spectre.Console;
using Azure.AI.OpenAI;
using System.ClientModel;
using System.Text.Json;
using OpenAI.Chat;

SpectreConsoleOutput.DisplayTitle("SK - AOAI");

// define video file and data folder
SpectreConsoleOutput.DisplayTitleH1("Video file and data folder");
string videoFile = VideosHelper.GetVideoFilePathCar();
string dataFolderPath = VideosHelper.CreateDataFolder();
Console.WriteLine();

var systemPrompt = PromptsHelper.SystemPrompt;
var userPrompt = PromptsHelper.UserPromptInsuranceCarAnalysis;

//////////////////////////////////////////////////////
/// VIDEO ANALYSIS using OpenCV
//////////////////////////////////////////////////////

SpectreConsoleOutput.DisplayTitleH1("Video Analysis using OpenCV");

// Extract the frames from the video
var video = new VideoCapture(videoFile);
var frames = new List<Mat>();
while (video.IsOpened())
{
    var frame = new Mat();
    if (!video.Read(frame) || frame.Empty())
        break;
    // resize the frame to half of its size if the with is greater than 800
    if (frame.Width > 800)
    {
        Cv2.Resize(frame, frame, new OpenCvSharp.Size(frame.Width / 2, frame.Height / 2));
    }
    frames.Add(frame);
}
video.Release();
SpectreConsoleOutput.DisplaySubtitle("Total Frames", frames.Count.ToString());
SpectreConsoleOutput.DisplayTitleH3("Video Analysis using OpenCV done!");


//////////////////////////////////////////////////////
/// Microsoft.Extensions.AI using Azure OpenAI
//////////////////////////////////////////////////////

SpectreConsoleOutput.DisplayTitleH1("Video Analysis using Semantic Kernel ans Azure OpenAI");
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AZURE_OPENAI_ENDPOINT"];
var modelId = config["AZURE_OPENAI_MODEL"];

// create client using API Keys
var apiKey = config["AZURE_OPENAI_APIKEY"];
var credential = new ApiKeyCredential(apiKey);
AzureOpenAIClient azureClient = new(new Uri(endpoint), credential);

var chatClient = azureClient.GetChatClient(modelId);

List<ChatMessage> messages =
[
    new SystemChatMessage(systemPrompt),
    new UserChatMessage(userPrompt),
];

// create the OpenAI files that represent the video frames
int step = (int)Math.Ceiling((double)frames.Count / PromptsHelper.NumberOfFrames);

// show the total number of frames and the step to get the desired number of frames using spectre console
SpectreConsoleOutput.DisplaySubtitle("Process", $"Get 1 frame every [{step}] to get the [{PromptsHelper.NumberOfFrames}] frames for analysis");

var tableImageAnalysis = new Table();
await AnsiConsole.Live(tableImageAnalysis)
    .AutoClear(false)   // Do not remove when done
    .Overflow(VerticalOverflow.Ellipsis) // Show ellipsis when overflowing
    .StartAsync(async ctx =>
    {
        tableImageAnalysis.AddColumn("N#");
        tableImageAnalysis.AddColumn("Location");
        ctx.Refresh();

        for (int i = 0; i < frames.Count; i += step)
        {
            // save the frame to the "data/frames" folder
            string framePath = Path.Combine(dataFolderPath, "frames", $"{i}.jpg");
            Cv2.ImWrite(framePath, frames[i]);

            // read the image bytes, create a new image content part and add it to the messages
            var imageContentPart = ChatMessageContentPart.CreateImagePart(
                imageBytes: BinaryData.FromBytes(File.ReadAllBytes(framePath)),
                imageBytesMediaType: "image/jpeg");
            var message = new UserChatMessage(imageContentPart);
            messages.Add(message);

            // add row
            tableImageAnalysis.AddRow(new Text(i.ToString()), new TextPath(framePath));
            ctx.Refresh();
        }
        ctx.Refresh();
    });

// display prompts
SpectreConsoleOutput.DisplayTablePrompts(systemPrompt, userPrompt);

SpectreConsoleOutput.DisplayTitleH1("Chat Client Response");


// send the messages to the assistant
var completeResponse = "";
AsyncCollectionResult<StreamingChatCompletionUpdate> completionUpdates = chatClient.CompleteChatStreamingAsync(messages);

// display the response
SpectreConsoleOutput.DisplayTitleH3("Azure OpenAI response using Semantic Kernel");

await foreach (StreamingChatCompletionUpdate completionUpdate in completionUpdates)
{
    if (completionUpdate.ContentUpdate.Count > 0)
    {
        AnsiConsole.Write(new Text(completionUpdate.ContentUpdate[0].Text));
        completeResponse += completionUpdate.ContentUpdate[0].Text;
    }
}

// validate if the complete response is a json object
if (IsValidJson(completeResponse))
{
    AnsiConsole.Write(
    new Panel(completeResponse)
        .Header("Response")
        .Collapse()
        .RoundedBorder()
        .BorderColor(Color.Yellow));
}

SpectreConsoleOutput.DisplayTitleH3("Video Analysis done!");

static bool IsValidJson(string jsonString)
{
    if (string.IsNullOrWhiteSpace(jsonString))
    {
        return false;
    }

    try
    {
        using (JsonDocument doc = JsonDocument.Parse(jsonString))
        {
            return true;
        }
    }
    catch (JsonException)
    {
        return false;
    }
}