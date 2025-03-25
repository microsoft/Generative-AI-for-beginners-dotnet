using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Azure.AI.Inference;
using Azure;
using Microsoft.Extensions.AI;
using MEAI.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Mscc.GenerativeAI.Microsoft;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
builder.Configuration.AddJsonFile("appsettings.LocalDebug.json", true, true);
#endif

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Embedding services
builder.Services.TryAddSingleton<IVectorStore, InMemoryVectorStore>();
//builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>, AzureAIInferenceEmbeddingGenerator>(serviceProvider =>
//{
//    var url = builder.Configuration.GetValue<string>("LLMConfig:Host");
//    var token = builder.Configuration.GetValue<string>("LLMConfig:Token");

//    var client = new EmbeddingsClient(new Uri(url), new AzureKeyCredential(token), new AzureAIInferenceClientOptions());

//    return new AzureAIInferenceEmbeddingGenerator(client);

//});
builder.Services.AddKeyedSingleton<IEmbeddingGenerator<string, Embedding<float>>, AzureAIInferenceEmbeddingGenerator>("AzureAI", (serviceProvider, key) =>
{
    var url = builder.Configuration.GetValue<string>("AzureAI:Host");
    var token = builder.Configuration.GetValue<string>("AzureAI:Token");

    var client = new EmbeddingsClient(new Uri(url), new AzureKeyCredential(token), new AzureAIInferenceClientOptions());

    return new AzureAIInferenceEmbeddingGenerator(client);
});
builder.Services.AddKeyedSingleton<IEmbeddingGenerator<string, Embedding<float>>, OllamaEmbeddingGenerator>("Ollama", (serviceProvider, key) =>
{
    var url = builder.Configuration.GetValue<string>("Ollama:Host");
    var model = builder.Configuration.GetValue<string>("Ollama:Model");

    return new OllamaEmbeddingGenerator(new Uri(url), model);
});
builder.Services.AddKeyedSingleton<IEmbeddingGenerator<string, Embedding<float>>, GeminiEmbeddingGenerator>("Gemini", (serviceProvider, key) =>
{
    var token = builder.Configuration.GetValue<string>("Gemini:Token");
    var model = builder.Configuration.GetValue<string>("Gemini:Model");

    return new GeminiEmbeddingGenerator(token, model);
});


builder.Services.AddSingleton<IMovieSearchService<int>, AIMovieSearchService<int>>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Hello world!");

app.Run();
