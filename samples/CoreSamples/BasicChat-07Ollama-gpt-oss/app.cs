#:package OllamaSharp@5.5.0
#:package Microsoft.Extensions.Configuration.UserSecrets@10.0.14

﻿using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Text;

// download the model from: https://ollama.com/library/gpt-oss:20b

IChatClient client =
    new OllamaApiClient(new Uri("http://localhost:11434/"), "gpt-oss:20b");

// Prompt to test a reasoning model capability
var prompt = new StringBuilder();
prompt.AppendLine("You are a helpful assistant.");
prompt.AppendLine("Answer the following question with a short explanation:");
prompt.AppendLine("Why is the sky blue?");

var response = await client.GetResponseAsync(prompt.ToString());

Console.WriteLine(response.Text);
