/*
 * Purpose: Demonstrate a caller that invokes the hosted agent with synthetic data.
 * Configuration identifies the hosted agent endpoint and agent, not the model deployment.
 */
using System.Text.Json;
using Azure.Identity;
using SupportRoutingCaller;

var endpoint = Environment.GetEnvironmentVariable("AGENT_OPENAI_BASE_URL")
    ?? throw new InvalidOperationException("Set AGENT_OPENAI_BASE_URL to the deployed agent's OpenAI-compatible base URL.");
var agentName = Environment.GetEnvironmentVariable("AGENT_NAME")
    ?? throw new InvalidOperationException("Set AGENT_NAME to the deployed hosted agent name.");
var ticket = args.Length == 0
    ? "I was charged twice for my subscription."
    : string.Join(' ', args);

using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
var credential = new AzureCliCredential();
var client = new ResponsesAgentClient(httpClient, credential, new Uri(endpoint), agentName);
var decision = await client.InvokeAsync(ticket);

Console.WriteLine(JsonSerializer.Serialize(
    decision,
    new JsonSerializerOptions(JsonSerializerDefaults.Web)));
