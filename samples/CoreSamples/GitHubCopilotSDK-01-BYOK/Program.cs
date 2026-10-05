using Azure.Core;
using Azure.Identity;
using GitHub.Copilot;
using Microsoft.Extensions.Configuration;

using var cancellation = new CancellationTokenSource(TimeSpan.FromMinutes(4));
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

try
{
    // User Secrets keeps the endpoint and credentials outside the repository.
    var configuration = new ConfigurationBuilder()
        .AddUserSecrets<Program>(optional: true)
        .Build();

    // This sample uses the repo's shared AzureOpenAI secrets.
    var modelName = GetRequiredValue(configuration, "AzureOpenAI:Deployment");
    var endpoint = GetRequiredValue(configuration, "AzureOpenAI:Endpoint");
    var apiKey = configuration["AzureOpenAI:ApiKey"]?.Trim();
    var question = args.Length > 0
        ? string.Join(' ', args)
        : "What is Microsoft Foundry, and how does it relate to the GitHub Copilot SDK?";

    // Microsoft Foundry exposes an API compatible with OpenAI Responses.
    var provider = new ProviderConfig
    {
        Type = "openai",
        BaseUrl = NormalizeEndpoint(endpoint),
        WireApi = "responses",
        ModelId = modelName,
        WireModel = modelName,
        MaxPromptTokens = 8000,
        MaxOutputTokens = 512
    };

    if (string.IsNullOrWhiteSpace(apiKey))
    {
        // AzureCliCredential reuses the session created with `az login`.
        var credential = new AzureCliCredential();
        var scope = GetTokenScope(endpoint);
#pragma warning disable GHCP001 // BYOK auth callbacks are experimental in the Copilot SDK.
        provider.BearerTokenProvider = async _ =>
        {
            var token = await credential.GetTokenAsync(
                new TokenRequestContext([scope]),
                cancellation.Token);
            return token.Token;
        };
#pragma warning restore GHCP001
    }
    else
    {
        provider.Headers = new Dictionary<string, string>
        {
            ["api-key"] = apiKey
        };
    }

    Console.WriteLine($"Model: {modelName}");
    Console.WriteLine(
        $"Authentication: {(string.IsNullOrWhiteSpace(apiKey) ? "AzureCliCredential" : "API key")}");
    Console.WriteLine($"Question: {question}");

    await using var client = new CopilotClient(new CopilotClientOptions
    {
        Connection = RuntimeConnection.ForStdio("copilot"),
        WorkingDirectory = AppContext.BaseDirectory
    });
    await client.StartAsync(cancellation.Token);

    // Disable tools, skills, and Git operations to isolate the model request.
    await using var session = await client.CreateSessionAsync(new SessionConfig
    {
        ClientName = "github-copilot-sdk-byok",
        Model = modelName,
        Provider = provider,
        SystemMessage = new SystemMessageConfig
        {
            Mode = SystemMessageMode.Replace,
            Content = "Answer every user question in English. Be concise and factual."
        },
        AvailableTools = [],
        Tools = [],
        EnableConfigDiscovery = false,
        EnableSkills = false,
        EnableHostGitOperations = false
    }, cancellation.Token);

    var response = await session.SendAndWaitAsync(
        new MessageOptions { Prompt = question },
        TimeSpan.FromMinutes(3),
        cancellation.Token);

    var content = response?.Data.Content;
    if (string.IsNullOrWhiteSpace(content))
    {
        throw new InvalidOperationException("The model returned an empty response.");
    }

    Console.WriteLine();
    Console.WriteLine("Answer:");
    Console.WriteLine(content);
}
catch (Exception ex) when (ex is AuthenticationFailedException or InvalidOperationException or
    ArgumentException or IOException or HttpRequestException or TimeoutException or
    OperationCanceledException)
{
    Console.Error.WriteLine($"[ERROR] {ex.GetType().Name}: {ex.Message}");
    Console.Error.WriteLine();
    Console.Error.WriteLine("Run the repository setup script from the root folder:");
    Console.Error.WriteLine("  ./setup-secrets.ps1 -Endpoint \"https://<resource>.services.ai.azure.com\"");
    Console.Error.WriteLine();
    Console.Error.WriteLine("Or configure the user secrets manually:");
    Console.Error.WriteLine("  dotnet user-secrets set \"AzureOpenAI:Endpoint\" \"https://<resource>.services.ai.azure.com\"");
    Console.Error.WriteLine("  dotnet user-secrets set \"AzureOpenAI:Deployment\" \"gpt-5-mini\"");
    Console.Error.WriteLine("  dotnet user-secrets set \"AzureOpenAI:ApiKey\" \"<optional-key>\"");
    Environment.ExitCode = 1;
}

static string GetRequiredValue(IConfiguration configuration, string key)
{
    var value = configuration[key]?.Trim();
    return string.IsNullOrWhiteSpace(value)
        ? throw new InvalidOperationException($"The required user secret '{key}' is missing.")
        : value;
}

static string GetTokenScope(string endpoint)
{
    // Azure OpenAI and Microsoft Foundry OpenAI-compatible inference endpoints are
    // Cognitive Services resources, so both authenticate with the same token scope.
    return Uri.TryCreate(endpoint.Trim(), UriKind.Absolute, out _)
        ? "https://cognitiveservices.azure.com/.default"
        : throw new ArgumentException("The configured endpoint must be an absolute URL.");
}

static string NormalizeEndpoint(string endpoint)
{
    var candidate = endpoint.Trim();
    if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri) ||
        uri.Scheme != Uri.UriSchemeHttps ||
        string.IsNullOrWhiteSpace(uri.Host))
    {
        throw new ArgumentException("The configured AzureOpenAI:Endpoint must be an HTTPS URL.");
    }

    // Accept the resource URL or a URL that already ends in /openai/v1.
    // Trimming first means a trailing slash does not need a separate check.
    var trimmed = candidate.TrimEnd('/');
    if (trimmed.EndsWith("/openai/v1", StringComparison.OrdinalIgnoreCase))
    {
        return trimmed + "/";
    }

    return $"{uri.GetLeftPart(UriPartial.Authority).TrimEnd('/')}/openai/v1/";
}
