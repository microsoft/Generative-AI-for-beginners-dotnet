// This is the same sample as Program.cs, but using Semantic Kernel
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicChat_05AIFoundryModels;

public class SKSample
{
    private const string SystemPrompt = "You are a useful chatbot. If you don't know an answer, say 'I don't know!'. Always reply in a funny way. Use emojis if possible.";
    public static async Task RunAsync()
    {
        try
        {
            var kernel = InitializeKernel();
            var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            await RunChatLoopAsync(chatCompletionService, kernel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    private static Kernel InitializeKernel()
    {
        var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
        var endpoint = config["endpoint"];
        var apiKey = config["apikey"];
        var deploymentName = config["deploymentName"];

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(deploymentName))
        {
            throw new InvalidOperationException("Azure OpenAI settings are missing in user secrets. Please check your configuration.");
        }

        var builder = Kernel.CreateBuilder();
        builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);
        return builder.Build();
    }

    private static async Task RunChatLoopAsync(IChatCompletionService chatCompletionService, Kernel kernel)
    {
        var history = new ChatHistory(SystemPrompt);

        while (true)
        {
            Console.Write("Q: ");
            var userQ = Console.ReadLine();
            if (string.IsNullOrEmpty(userQ))
            {
                break;
            }
            history.AddUserMessage(userQ);

            var sb = new StringBuilder();
            var result = chatCompletionService.GetStreamingChatMessageContentsAsync(history, kernel: kernel);
            
            Console.Write("AI: ");
            await foreach (var item in result)
            {
                sb.Append(item.Content);
                Console.Write(item.Content);
            }
            Console.WriteLine();

            history.AddAssistantMessage(sb.ToString());
        }
    }
}
