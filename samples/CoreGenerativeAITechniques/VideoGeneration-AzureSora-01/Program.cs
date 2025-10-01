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
string model = "sora";
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
string videoFile = await Sora(prompt, 480, 480, 5);
Console.WriteLine($"Generated video: {videoFile}");

async Task<string> Sora(string prompt, int width = 480, int height = 480, int nSeconds = 5)
{
    var start = DateTime.Now;
    string apiVersion = "preview";
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("api-key", apiKey);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    string idx = DateTime.Now.ToString("ddMMMyyyy_HHmmss");
    string suffix = new string(prompt.Length > 30 ? prompt.Substring(0, 30).ToCharArray() : prompt.ToCharArray());
    suffix = suffix.Replace(",", "_").Replace(".", "_").Replace(" ", "_");
    string outputFilename = Path.Combine(outputDir, $"sora_{idx}_{suffix}.mp4");

    // 1. Create a video generation job
    string createUrl = $"{endpoint}/openai/v1/video/generations/jobs?api-version={apiVersion}";
    var body = new
    {
        prompt = prompt,
        width = width,
        height = height,
        n_seconds = nSeconds,
        model = model
    };
    var bodyJson = JsonSerializer.Serialize(body);
    var response = await client.PostAsync(createUrl, new StringContent(bodyJson, Encoding.UTF8, "application/json"));
    response.EnsureSuccessStatusCode();
    var responseJson = await response.Content.ReadAsStringAsync();
    Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Full response JSON: {responseJson}\n");

    using var doc = JsonDocument.Parse(responseJson);
    string jobId = doc.RootElement.GetProperty("id").GetString();
    Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Job created: {jobId}");

    // 2. Poll for job status
    string statusUrl = $"{endpoint}/openai/v1/video/generations/jobs/{jobId}?api-version={apiVersion}";
    string status = null;
    JsonElement statusResponse = default;
    do
    {
        await Task.Delay(5000);
        var statusResp = await client.GetAsync(statusUrl);
        var statusJson = await statusResp.Content.ReadAsStringAsync();
        var statusDoc = JsonDocument.Parse(statusJson);
        statusResponse = statusDoc.RootElement;
        status = statusResponse.GetProperty("status").GetString();
        Console.WriteLine($"{DateTime.Now:dd-MMM-yyyy HH:mm:ss} Job status: {status}");
    } while (status != "succeeded" && status != "failed" && status != "cancelled");

    // 3. Retrieve generated video
    if (status == "succeeded")
    {
        if (statusResponse.TryGetProperty("generations", out JsonElement generations) && generations.GetArrayLength() > 0)
        {
            Console.WriteLine($"\n{DateTime.Now:dd-MMM-yyyy HH:mm:ss} ✅ Done. Video generation succeeded.\n");
            string generationId = generations[0].GetProperty("id").GetString();
            string videoUrl = $"{endpoint}/openai/v1/video/generations/{generationId}/content/video?api-version={apiVersion}";
            var videoResp = await client.GetAsync(videoUrl);
            if (videoResp.IsSuccessStatusCode)
            {
                Console.WriteLine("\nDownloading the video...");
                using (var fs = new FileStream(outputFilename, FileMode.Create, FileAccess.Write))
                {
                    await videoResp.Content.CopyToAsync(fs);
                }
                Console.WriteLine($"SORA Generated video saved: '{outputFilename}'");
                var elapsed = DateTime.Now - start;
                Console.WriteLine($"Done in {elapsed.Minutes} minutes and {elapsed.Seconds} seconds");
                return outputFilename;
            }
            else
            {
                throw new Exception("Error downloading video content.");
            }
        }
        else
        {
            throw new Exception("Error. No generations found in job result.");
        }
    }
    else
    {
        throw new Exception($"Error. Job did not succeed. Status: {status}");
    }
}