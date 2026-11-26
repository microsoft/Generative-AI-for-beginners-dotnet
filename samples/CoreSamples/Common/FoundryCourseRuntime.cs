using Microsoft.AI.Foundry.Local;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using OpenAI;
using System.ClientModel;

namespace FoundryLocal.Course;

internal sealed class FoundryCourseRuntime : IAsyncDisposable
{
    private readonly FoundryLocalManager manager;
    private readonly List<IModel> loaded = [];
    private IModel? chatModel;
    private OpenAIClient? client;
    private bool serving;

    private FoundryCourseRuntime(FoundryLocalManager manager) => this.manager = manager;

    public IModel ChatModel => chatModel ?? throw new InvalidOperationException("No chat model was loaded.");
    public IModel? EmbeddingModel { get; private set; }
    public OpenAIClient Client => client ?? throw new InvalidOperationException("This runtime has no HTTP server.");

    public static async Task<FoundryCourseRuntime> StartAsync(
        bool prepare, bool embeddings = false, bool server = true, bool tools = false)
    {
        string alias = ReadModel("FOUNDRY_LOCAL_MODEL", "phi-4-mini");
        Environment.SetEnvironmentVariable("ORT_TELEMETRY_DISABLED", "1");
        await FoundryLocalManager.CreateAsync(new Configuration
        {
            AppName = "dotnet-beginners-foundry",
            DisableNonessentialTelemetry = true,
            UseSharedHost = false,
            Web = new Configuration.WebService { Urls = "http://127.0.0.1:0" }
        }, NullLogger.Instance);
        var runtime = new FoundryCourseRuntime(FoundryLocalManager.Instance);
        try
        {
            var catalog = await runtime.manager.GetCatalogAsync();
            runtime.chatModel = await catalog.GetModelAsync(alias)
                ?? throw new InvalidOperationException($"Chat model '{alias}' was not found. No fallback is selected.");
            ValidateModel(runtime.ChatModel.Info.Task, runtime.ChatModel.Info.SupportsToolCalling, false, tools);
            await runtime.LoadAsync(runtime.ChatModel, prepare);
            if (embeddings)
            {
                string embeddingAlias = ReadModel("FOUNDRY_LOCAL_EMBEDDING_MODEL", "qwen3-embedding-0.6b");
                runtime.EmbeddingModel = await catalog.GetModelAsync(embeddingAlias)
                    ?? throw new InvalidOperationException($"Embedding model '{embeddingAlias}' was not found.");
                ValidateModel(runtime.EmbeddingModel.Info.Task, false, true, false);
                await runtime.LoadAsync(runtime.EmbeddingModel, prepare);
            }
            if (server)
            {
                await runtime.manager.StartWebServiceAsync();
                runtime.serving = true;
                string url = runtime.manager.Urls?.FirstOrDefault()
                    ?? throw new InvalidOperationException("Foundry Local did not report its bound address.");
                var endpoint = new Uri(url.TrimEnd('/') + "/v1");
                if (!endpoint.IsLoopback || endpoint.Scheme != "http")
                {
                    throw new InvalidOperationException("The course server must bind to HTTP loopback.");
                }
                runtime.client = new OpenAIClient(new ApiKeyCredential("not-needed"),
                    new OpenAIClientOptions { Endpoint = endpoint });
                Console.WriteLine($"Local endpoint: {endpoint}");
            }
            return runtime;
        }
        catch
        {
            await runtime.DisposeAsync();
            throw;
        }
    }

    public IChatClient CreateChatClient() => Client.GetChatClient(ChatModel.Id).AsIChatClient();

    public IEmbeddingGenerator<string, Embedding<float>> CreateEmbeddingGenerator() =>
        Client.GetEmbeddingClient(EmbeddingModel?.Id
            ?? throw new InvalidOperationException("No embedding model was loaded.")).AsIEmbeddingGenerator();

    internal static string ReadModel(string variable, string defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(variable);
        if (value is null)
        {
            return defaultValue;
        }
        return !string.IsNullOrWhiteSpace(value)
            ? value.Trim()
            : throw new ArgumentException($"{variable} must not be empty.");
    }

    internal static void ValidateModel(string? task, bool? supportsTools, bool embeddings, bool tools)
    {
        if (embeddings ? task != "embeddings" : task is not ("chat-completion" or "vision-language-chat"))
        {
            throw new InvalidOperationException($"Wrong model task '{task}'; expected {(embeddings ? "embeddings" : "chat")}.");
        }

        if (tools && supportsTools != true)
        {
            throw new InvalidOperationException("The selected model does not advertise tool calling.");
        }
    }

    internal static void ValidateCache(bool cached, bool prepare, string modelId)
    {
        if (!cached && !prepare)
        {
            throw new InvalidOperationException(
                $"Model '{modelId}' is not cached. Run this sample with --prepare to allow its download.");
        }
    }

    private async Task LoadAsync(IModel model, bool prepare)
    {
        bool cached = await model.IsCachedAsync();
        Console.WriteLine($"Model: {model.Id}; task: {model.Info.Task}; cached: {cached}");
        ValidateCache(cached, prepare, model.Id);
        if (!cached)
        {
            Console.WriteLine($"Downloading {model.Id}. The cache is retained after this sample exits.");
            await model.DownloadAsync(progress => Console.Write($"\rDownload: {progress:F1}% "));
            Console.WriteLine();
        }
        await model.LoadAsync();
        loaded.Add(model);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (serving)
            {
                await manager.StopWebServiceAsync();
                serving = false;
            }
        }
        finally
        {
            try
            {
                foreach (var model in loaded.AsEnumerable().Reverse())
                {
                    await model.UnloadAsync();
                }
                loaded.Clear();
            }
            finally
            {
                manager.Dispose();
            }
        }
    }
}
