using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AzFunction_BasicChat_01;

public sealed class ChatFunction(
    IChatResponder chatResponder,
    IOptions<FoundryOptions> options,
    ILogger<ChatFunction> logger)
{
    public const int MaximumQuestionLength = 4_000;

    [Function("Chat")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chat")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        ChatRequest? chatRequest;

        try
        {
            chatRequest = await JsonSerializer.DeserializeAsync<ChatRequest>(
                request.Body,
                JsonSerializerOptions.Web,
                cancellationToken: cancellationToken);
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(new ErrorResponse("The request body must be valid JSON."));
        }

        if (string.IsNullOrWhiteSpace(chatRequest?.Question))
        {
            return new BadRequestObjectResult(new ErrorResponse("The question field is required."));
        }

        if (chatRequest.Question.Length > MaximumQuestionLength)
        {
            return new BadRequestObjectResult(
                new ErrorResponse($"The question field must not exceed {MaximumQuestionLength} characters."));
        }

        logger.LogInformation(
            "Sending a one-shot chat request to deployment {DeploymentName}.",
            options.Value.DeploymentName);

        var answer = await chatResponder.GetAnswerAsync(chatRequest.Question, cancellationToken);

        return new OkObjectResult(
            new ChatResponse(chatRequest.Question, answer, options.Value.DeploymentName));
    }
}
