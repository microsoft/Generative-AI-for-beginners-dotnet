// based on https://github.com/retkowsky/Azure-OpenAI-demos/blob/main/sora/SORA%20with%20Azure%20AI%20Foundry.ipynb

using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

// load endpoint and api_key values
var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
var configuration = builder.Build();
string endpoint = configuration["endpoint"];
string apiKey = configuration["api_key"];
string model = "sora-2";
string outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "sora_videos");

if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Please set the endpoint and apikey as user secrets in this project.");
    return;
}

// prompt
string prompt = "Two puppies playing soccer in the moon. Use a cartoon style.";

Directory.CreateDirectory(outputDir);
Console.WriteLine($"Today is {DateTime.Now:dd-MMM-yyyy HH:mm:ss}");

// run
string videoFile = await Sora(prompt, "720x1280", 4);
Console.WriteLine($"Generated video: {videoFile}");

async Task<string> Sora(string prompt, string size = "720x1280", int seconds = 4)
{
    var start = DateTime.Now;
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("Api-key", apiKey);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    string idx = DateTime.Now.ToString("ddMMMyyyy_HHmmss");
    string suffix = new string(prompt.Length > 30 ? prompt.Substring(0, 30).ToCharArray() : prompt.ToCharArray());
    suffix = suffix.Replace(",", "_").Replace(".", "_").Replace(" ", "_");
    string outputFilename = Path.Combine(outputDir, $"sora_{idx}_{suffix}.mp4");

    // 1. Create a video generation job
    Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Sending video generation request...");
    var body = new
    {
        model = model,
        prompt = prompt,
        size = size,
        seconds = seconds.ToString()
    };
    var bodyJson = JsonSerializer.Serialize(body);
    Console.WriteLine($"Request body: {bodyJson}\n");
    
    var response = await client.PostAsync(endpoint, new StringContent(bodyJson, Encoding.UTF8, "application/json"));
    response.EnsureSuccessStatusCode();
    var responseJson = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Job created response: {responseJson}\n");

    // Parse the job creation response
    using var createDoc = JsonDocument.Parse(responseJson);
    var root = createDoc.RootElement;
    
    string videoId = root.GetProperty("id").GetString();
    string status = root.GetProperty("status").GetString();
    
    Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Video job created with ID: {videoId}");
    Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Initial status: {status}\n");

    // 2. Poll for job completion
    string pollUrl = $"{endpoint}/{videoId}";
    JsonElement statusResponse = default;
    
    while (status == "queued" || status == "in_progress")
    {
        await Task.Delay(5000); // Poll every 5 seconds
        
        var pollResp = await client.GetAsync(pollUrl);
        pollResp.EnsureSuccessStatusCode();
        var pollJson = await pollResp.Content.ReadAsStringAsync();
        
        var pollDoc = JsonDocument.Parse(pollJson);
        statusResponse = pollDoc.RootElement;
        status = statusResponse.GetProperty("status").GetString();
        
        int progress = statusResponse.GetProperty("progress").GetInt32();
        Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Job status: {status} (Progress: {progress}%)");
        
        pollDoc.Dispose();
    }

    // 3. Handle completion or error
    if (status == "completed")
    {
        Console.WriteLine($"\n{DateTime.Now:dd-MMM-yyyy HH:mm:ss} ✅ Video generation completed!\n");
        
        // Download the video - the video content should be available at the job endpoint with /content suffix
        string videoContentUrl = $"{endpoint}/{videoId}/content";
        Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Downloading video from: {videoContentUrl}");
        
        var videoResp = await client.GetAsync(videoContentUrl);
        if (videoResp.IsSuccessStatusCode)
        {
            Console.WriteLine("\nDownloading the video...");
            using (var fs = new FileStream(outputFilename, FileMode.Create, FileAccess.Write))
            {
                await videoResp.Content.CopyToAsync(fs);
            }
            Console.WriteLine($"✅ SORA Generated video saved: '{outputFilename}'");
            var elapsed = DateTime.Now - start;
            Console.WriteLine($"Done in {elapsed.Minutes} minutes and {elapsed.Seconds} seconds");
            return outputFilename;
        }
        else
        {
            throw new Exception($"Error downloading video content. Status: {videoResp.StatusCode}");
        }
    }
    else if (status == "failed")
    {
        string errorMsg = "Unknown error";
        if (statusResponse.TryGetProperty("error", out JsonElement errorElement) && errorElement.ValueKind != JsonValueKind.Null)
        {
            errorMsg = errorElement.ToString();
        }
        throw new Exception($"Video generation failed. Error: {errorMsg}");
    }
    else if (status == "expired")
    {
        throw new Exception("Video generation job expired.");
    }
    else
    {
        throw new Exception($"Unexpected job status: {status}");
    }
}