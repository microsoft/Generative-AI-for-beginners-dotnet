using Microsoft.Extensions.Configuration;
using OpenCvSharp;
using Azure.AI.OpenAI;
using System.ClientModel;
using Microsoft.Extensions.AI;
using System;


// define video file and data folder
string videoFile = VideosHelper.GetVideoFilePathFireTruck();
string dataFolderPath = VideosHelper.CreateDataFolder();

//////////////////////////////////////////////////////
/// VIDEO ANALYSIS using OpenCV
//////////////////////////////////////////////////////

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


var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var endpoint = config["AZURE_OPENAI_ENDPOINT"];
var modelId = config["AZURE_OPENAI_MODEL"];

// create client using API Keys
var apiKey = config["AZURE_OPENAI_APIKEY"];
var credential = new ApiKeyCredential(apiKey);

IChatClient chatClient =
    new AzureOpenAIClient(new Uri(endpoint),credential)
            .GetChatClient(modelId)
            .AsIChatClient();

List<ChatMessage> messages =
[
    new ChatMessage(ChatRole.System, PromptsHelper.SystemPrompt),
    new ChatMessage(ChatRole.User, PromptsHelper.UserPromptDescribeVideo),
];


// create the OpenAI files that represent the video frames
int step = (int)Math.Ceiling((double)frames.Count / PromptsHelper.NumberOfFrames);

// show in the console the total number of frames and the step that neeeds to be taken to get the desired number of frames for the video analysis
Console.WriteLine($"Video total number of frames: {frames.Count}");
Console.WriteLine($"Get 1 frame every [{step}] to get the [{PromptsHelper.NumberOfFrames}] frames for analysis");

for (int i = 0; i < frames.Count; i += step)
{
    // save the frame to the "data/frames" folder
    string framePath = Path.Combine(dataFolderPath, "frames", $"{i}.jpg");
    Cv2.ImWrite(framePath, frames[i]);

    // read the image bytes, create a new image content part and add it to the messages
    AIContent aic = new DataContent(File.ReadAllBytes(framePath), "image/jpeg");
    var message = new ChatMessage(ChatRole.User, [aic]);
    messages.Add(message);
}

// send the messages to the chat client
var completionUpdates = chatClient.GetStreamingResponseAsync(messages);

// print the assistant responses
Console.WriteLine($"\n[Azure OpenAI Services response using Microsoft Extensions for AI]: ");
await foreach (var completionUpdate in completionUpdates)
{
    if (completionUpdate.Contents.Count > 0)
    {
        Console.Write(completionUpdate.Contents[0]);
    }
}