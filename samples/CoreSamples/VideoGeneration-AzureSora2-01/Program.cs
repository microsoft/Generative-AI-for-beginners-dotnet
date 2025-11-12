// based on https://github.com/retkowsky/Azure-OpenAI-demos/blob/main/sora/SORA%20with%20Azure%20AI%20Foundry.ipynb

using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
    string outputFilename = Path.Combine(outputDir, $"sora-2_{idx}_{suffix}.mp4");

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

    // Parse the job creation response using strongly-typed class
    var videoJob = JsonSerializer.Deserialize<VideoJobResponse>(responseJson, new JsonSerializerOptions 
    { 
        PropertyNameCaseInsensitive = true 
    });
    
    if (videoJob == null)
    {
        throw new Exception("Failed to deserialize job creation response");
    }
    
    Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Video job created with ID: {videoJob.Id}");
    Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Initial status: {videoJob.Status}\n");

    // 2. Poll for job completion
    string pollUrl = $"{endpoint}/{videoJob.Id}";
    
    while (videoJob.Status == "queued" || videoJob.Status == "in_progress")
    {
        await Task.Delay(5000); // Poll every 5 seconds
        
        var pollResp = await client.GetAsync(pollUrl);
        pollResp.EnsureSuccessStatusCode();
        var pollJson = await pollResp.Content.ReadAsStringAsync();
        
        videoJob = JsonSerializer.Deserialize<VideoJobResponse>(pollJson, new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        });
        
        if (videoJob == null)
        {
            throw new Exception("Failed to deserialize polling response");
        }
        
        Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Job status: {videoJob.Status} (Progress: {videoJob.Progress}%)");
    }

    // 3. Handle completion or error
    if (videoJob.Status == "completed")
    {
        Console.WriteLine($"\n{DateTime.Now:dd-MMM-yyyy HH:mm:ss} ✅ Video generation completed!\n");
        
        if (videoJob.CompletedAt.HasValue)
        {
            var completedTime = DateTimeOffset.FromUnixTimeSeconds(videoJob.CompletedAt.Value).DateTime;
            Console.WriteLine($"Completed at: {completedTime:dd-MMM-yyyy HH:mm:ss}");
        }
        
        // Download the video - the video content should be available at the job endpoint with /content suffix
        string videoContentUrl = $"{endpoint}/{videoJob.Id}/content";
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
    else if (videoJob.Status == "failed")
    {
        string errorMsg = videoJob.Error?.Message ?? "Unknown error";
        string errorCode = videoJob.Error?.Code ?? "unknown_code";
        throw new Exception($"Video generation failed. Error [{errorCode}]: {errorMsg}");
    }
    else if (videoJob.Status == "expired")
    {
        var expiredTime = videoJob.ExpiresAt.HasValue 
            ? DateTimeOffset.FromUnixTimeSeconds(videoJob.ExpiresAt.Value).DateTime.ToString("dd-MMM-yyyy HH:mm:ss")
            : "unknown time";
        throw new Exception($"Video generation job expired at {expiredTime}.");
    }
    else
    {
        throw new Exception($"Unexpected job status: {videoJob.Status}");
    }
}

// Video job response model
record VideoJobError
{
    [JsonPropertyName("code")]
    public string Code { get; init; } = string.Empty;
    
    [JsonPropertyName("message")]
    public string Message { get; init; } = string.Empty;
}

record VideoJobResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;
    
    [JsonPropertyName("object")]
    public string Object { get; init; } = string.Empty;
    
    [JsonPropertyName("created_at")]
    public long CreatedAt { get; init; }
    
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;
    
    [JsonPropertyName("completed_at")]
    public long? CompletedAt { get; init; }
    
    [JsonPropertyName("error")]
    public VideoJobError? Error { get; init; }
    
    [JsonPropertyName("expires_at")]
    public long? ExpiresAt { get; init; }
    
    [JsonPropertyName("model")]
    public string Model { get; init; } = string.Empty;
    
    [JsonPropertyName("progress")]
    public int Progress { get; init; }
    
    [JsonPropertyName("remixed_from_video_id")]
    public string? RemixedFromVideoId { get; init; }
    
    [JsonPropertyName("seconds")]
    public string Seconds { get; init; } = string.Empty;
    
    [JsonPropertyName("size")]
    public string Size { get; init; } = string.Empty;
}