// based on https://github.com/retkowsky/Azure-OpenAI-demos/blob/main/sora/SORA%20with%20Azure%20AI%20Foundry.ipynb

using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

// All samples in this repo share the user-secrets store "genai-beginners-dotnet", so this sample
// reads its own scoped "Sora:*" keys first and only then falls back to the old generic key names.
var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
var configuration = builder.Build();
string? endpoint = configuration["Sora:Endpoint"] ?? configuration["endpoint"];
string? apiKey = configuration["Sora:ApiKey"] ?? configuration["api_key"];
string model = configuration["Sora:Deployment"] ?? configuration["deploymentName"] ?? "sora-2";
string outputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "sora_videos");

var missing = new List<string>();
if (string.IsNullOrWhiteSpace(endpoint)) missing.Add("Sora:Endpoint");
if (string.IsNullOrWhiteSpace(apiKey)) missing.Add("Sora:ApiKey");

if (missing.Count > 0)
{
    Console.WriteLine($"""
        Azure Sora video generation is not configured. Missing: {string.Join(", ", missing)}

        Prerequisite: a Sora video deployment in Azure AI Foundry (currently the available model
        is "sora-2"). Create it in the Azure AI Foundry portal under Deployments, in a region that
        offers Sora. Without that deployment these keys cannot be filled in.

        Then set the missing values (all samples share the user-secrets id "genai-beginners-dotnet"):

        dotnet user-secrets set --id genai-beginners-dotnet "Sora:Endpoint" "https://<your-resource>.openai.azure.com"
        dotnet user-secrets set --id genai-beginners-dotnet "Sora:ApiKey" "<your-api-key>"
        dotnet user-secrets set --id genai-beginners-dotnet "Sora:Deployment" "sora-2"

        Sora:Deployment is optional and defaults to "sora-2".
        The legacy keys "endpoint" and "api_key" still work, but they are shared with other
        samples in this store, so the scoped "Sora:*" keys are recommended.
        """);
    return 1;
}

// prompt
string prompt = "Two puppies playing soccer in the moon. Use a cartoon style.";

Directory.CreateDirectory(outputDir);
Console.WriteLine($"Today is {DateTime.Now:dd-MMM-yyyy HH:mm:ss}");

// run
string videoFile = await Sora(prompt, 480, 480, 5);
Console.WriteLine($"Generated video: {videoFile}");
return 0;

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