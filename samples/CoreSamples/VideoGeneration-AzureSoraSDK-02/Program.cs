using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureSoraSDK;
using AzureSoraSDK.Configuration;
using AzureSoraSDK.Exceptions;
using AzureSoraSDK.Extensions;
using AzureSoraSDK.Interfaces;
using AzureSoraSDK.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VideoGeneration_AzureSoraSDK_02
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Azure SORA SDK Console Sample ===\n");

            // Load configuration from user secrets
            var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
            var configuration = builder.Build();
            
            string endpoint = configuration["endpoint"] ?? "https://your-resource.openai.azure.com";
            string apiKey = configuration["api_key"] ?? "your-api-key";
            string deploymentName = configuration["deployment_name"] ?? "sora";

            if (string.IsNullOrWhiteSpace(apiKey) || apiKey == "your-api-key")
            {
                Console.WriteLine("Please set the endpoint, api_key, and deployment_name as user secrets in this project.");
                Console.WriteLine("Use the following commands:");
                Console.WriteLine("dotnet user-secrets set \"endpoint\" \"https://your-resource.openai.azure.com\"");
                Console.WriteLine("dotnet user-secrets set \"api_key\" \"your-api-key\"");
                Console.WriteLine("dotnet user-secrets set \"deployment_name\" \"sora\"");
                return;
            }

            Console.WriteLine("Choose an example to run:");
            Console.WriteLine("1. Basic SDK Usage Example");
            Console.WriteLine("2. Dependency Injection Example");
            Console.Write("Enter your choice (1 or 2): ");
            
            var choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    await BasicSDKExample(endpoint, apiKey, deploymentName);
                    break;
                case "2":
                    await DependencyInjectionExample(endpoint, apiKey, deploymentName);
                    break;
                default:
                    Console.WriteLine("Invalid choice, running basic example...");
                    await BasicSDKExample(endpoint, apiKey, deploymentName);
                    break;
            }
        }

        static async Task BasicSDKExample(string endpoint, string apiKey, string deploymentName)
        {
            Console.WriteLine("\n=== Basic SDK Usage Example ===\n");

            // Configure the client
            var options = new SoraClientOptions
            {
                Endpoint = endpoint,
                ApiKey = apiKey,
                DeploymentName = deploymentName,
                ApiVersion = "preview"
            };

            // Create client
            using var client = new SoraClient(options.Endpoint, options.ApiKey, options.DeploymentName);

            try
            {
                // Example 1: Submit video generation job with explicit dimensions
                Console.WriteLine("Generating video with explicit dimensions...");
                var jobId1 = await client.SubmitVideoJobAsync(
                    prompt: "A serene waterfall in a lush forest with sunlight filtering through trees",
                    width: 1280,
                    height: 720,
                    nSeconds: 10);

                Console.WriteLine($"Job submitted: {jobId1}");

                // Example 2: Submit video generation job with aspect ratio (v1.0.2+)
                Console.WriteLine("\nGenerating video with aspect ratio...");
                var jobId2 = await client.SubmitVideoJobAsync(
                    prompt: "A futuristic city at night with flying cars and neon lights",
                    aspectRatio: "16:9",
                    quality: "high",
                    nSeconds: 15);

                Console.WriteLine($"Job submitted: {jobId2}");

                // Wait for completion of first job
                Console.WriteLine($"\nWaiting for job {jobId1} to complete...");
                var videoUri = await client.WaitForCompletionAsync(jobId1);
                Console.WriteLine($"Video ready: {videoUri}");

                // Download video
                var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "sora_videos", "waterfall.mp4");
                Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
                await client.DownloadVideoAsync(videoUri, outputPath);
                Console.WriteLine($"Video downloaded to: {outputPath}");

                // Check status of second job
                var status = await client.GetJobStatusAsync(jobId2);
                Console.WriteLine($"\nJob {jobId2} status: {status.Status}");

                // Example 3: Calculate custom dimensions
                Console.WriteLine("\nDimension calculation examples:");
                
                var (width, height) = SoraClient.GetCommonDimensions("1:1", "medium");
                Console.WriteLine($"Square video (medium): {width}x{height}");
                
                var (w2, h2) = SoraClient.CalculateDimensionsFromAspectRatio("2.35:1", 1920);
                Console.WriteLine($"Cinema format: {w2}x{h2}");
            }
            catch (SoraValidationException ex)
            {
                Console.WriteLine($"Validation Error: {ex.Message}");
                if (ex.ValidationErrors != null)
                {
                    foreach (var error in ex.ValidationErrors)
                    {
                        Console.WriteLine($"Validation error in {error.Key}: {string.Join(", ", error.Value)}");
                    }
                }
            }
            catch (SoraRateLimitException ex)
            {
                Console.WriteLine($"Rate Limit Error: {ex.Message}. Retry after: {ex.RetryAfter}");
            }
            catch (SoraTimeoutException ex)
            {
                Console.WriteLine($"Timeout Error: {ex.Message}. Timeout: {ex.Timeout}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static async Task DependencyInjectionExample(string endpoint, string apiKey, string deploymentName)
        {
            Console.WriteLine("\n=== Dependency Injection Example ===\n");

            // Build the host
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Configure Sora SDK
                    services.AddAzureSoraSDK(options =>
                    {
                        options.Endpoint = endpoint;
                        options.ApiKey = apiKey;
                        options.DeploymentName = deploymentName;
                        options.MaxRetryAttempts = 5;
                        options.HttpTimeout = TimeSpan.FromMinutes(10);
                    });

                    // Add our example service
                    services.AddTransient<VideoGenerationService>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .Build();

            // Get the service and run it
            var videoService = host.Services.GetRequiredService<VideoGenerationService>();
            await videoService.RunAsync();
        }
    }

    public class VideoGenerationService
    {
        private readonly ISoraClient _soraClient;
        private readonly IPromptEnhancer _promptEnhancer;
        private readonly ILogger<VideoGenerationService> _logger;

        public VideoGenerationService(
            ISoraClient soraClient,
            IPromptEnhancer promptEnhancer,
            ILogger<VideoGenerationService> logger)
        {
            _soraClient = soraClient;
            _promptEnhancer = promptEnhancer;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            try
            {
                // Original prompt
                var originalPrompt = "A forest scene with magical creatures playing among ancient trees";
                _logger.LogInformation("Original prompt: {Prompt}", originalPrompt);

                // Get enhanced prompts
                _logger.LogInformation("Getting prompt suggestions...");
                var suggestions = await _promptEnhancer.SuggestPromptsAsync(originalPrompt, 3);
                
                Console.WriteLine("\nPrompt suggestions:");
                for (int i = 0; i < suggestions.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {suggestions[i]}");
                }

                // Use the first suggestion or original if none
                var enhancedPrompt = suggestions.Length > 0 ? suggestions[0] : originalPrompt;
                _logger.LogInformation("Using prompt: {Prompt}", enhancedPrompt);

                // Submit video job with validated request
                var request = new VideoGenerationRequest
                {
                    Prompt = enhancedPrompt,
                    Width = 1920,
                    Height = 1080,
                    NSeconds = 15,
                    Metadata = new Dictionary<string, string>
                    {
                        ["source"] = "example-app",
                        ["version"] = "1.0",
                        ["timestamp"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }
                };

                // Validate the request
                request.Validate();

                _logger.LogInformation("Submitting video generation job...");
                var jobId = await _soraClient.SubmitVideoJobAsync(
                    request.Prompt,
                    request.Width,
                    request.Height,
                    request.NSeconds
                );

                _logger.LogInformation("Job submitted: {JobId}", jobId);

                // Monitor progress
                var completed = false;
                while (!completed)
                {
                    var details = await _soraClient.GetJobStatusAsync(jobId);
                    
                    _logger.LogInformation(
                        "Job {JobId} status: {Status} (Progress: {Progress}%)",
                        jobId,
                        details.Status,
                        details.ProgressPercentage ?? 0
                    );

                    switch (details.Status)
                    {
                        case JobStatus.Succeeded:
                            Console.WriteLine($"\nVideo generated successfully!");
                            Console.WriteLine($"Video URL: {details.VideoUrl}");
                            
                            // Download the video
                            var outputPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "sora_videos", "forest_scene.mp4");
                            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
                            await _soraClient.DownloadVideoAsync(new Uri(details.VideoUrl!), outputPath);
                            Console.WriteLine($"Video downloaded to: {outputPath}");
                            
                            completed = true;
                            break;

                        case JobStatus.Failed:
                            _logger.LogError(
                                "Job failed: {ErrorMessage} (Code: {ErrorCode})",
                                details.ErrorMessage,
                                details.ErrorCode
                            );
                            completed = true;
                            break;

                        case JobStatus.Cancelled:
                            _logger.LogWarning("Job was cancelled");
                            completed = true;
                            break;

                        default:
                            await Task.Delay(TimeSpan.FromSeconds(5));
                            break;
                    }
                }
            }
            catch (SoraValidationException ex)
            {
                _logger.LogError(ex, "Validation error occurred");
                if (ex.ValidationErrors != null)
                {
                    foreach (var error in ex.ValidationErrors)
                    {
                        Console.WriteLine($"Validation error in {error.Key}: {string.Join(", ", error.Value)}");
                    }
                }
            }
            catch (SoraRateLimitException ex)
            {
                _logger.LogWarning(ex, "Rate limit exceeded. Retry after: {RetryAfter}", ex.RetryAfter);
            }
            catch (SoraTimeoutException ex)
            {
                _logger.LogError(ex, "Operation timed out after {Timeout}", ex.Timeout);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred");
            }
        }
    }
} 