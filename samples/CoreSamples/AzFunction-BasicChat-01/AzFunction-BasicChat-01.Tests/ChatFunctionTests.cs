using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace AzFunction_BasicChat_01.Tests;

public sealed class ChatFunctionTests
{
    private const string DeploymentName = "test-deployment";

    [Fact]
    public async Task RunAsync_WithQuestion_ReturnsModelAnswer()
    {
        var responder = new FakeChatResponder("Retrieval-augmented generation adds external context.");
        var function = CreateFunction(responder);
        var request = CreateRequest("""{"question":"What is RAG?"}""");

        var result = await function.RunAsync(request, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<ChatResponse>(okResult.Value);
        Assert.Equal("What is RAG?", response.Question);
        Assert.Equal("Retrieval-augmented generation adds external context.", response.Answer);
        Assert.Equal(DeploymentName, response.Model);
        Assert.Equal("What is RAG?", responder.LastQuestion);
    }

    [Fact]
    public async Task RunAsync_WithMalformedJson_ReturnsBadRequest()
    {
        var function = CreateFunction(new FakeChatResponder("unused"));
        var request = CreateRequest("""{"question":""");

        var result = await function.RunAsync(request, CancellationToken.None);

        AssertError(result, "The request body must be valid JSON.");
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("""{"question":null}""")]
    [InlineData("""{"question":"   "}""")]
    public async Task RunAsync_WithoutQuestion_ReturnsBadRequest(string body)
    {
        var function = CreateFunction(new FakeChatResponder("unused"));
        var request = CreateRequest(body);

        var result = await function.RunAsync(request, CancellationToken.None);

        AssertError(result, "The question field is required.");
    }

    [Fact]
    public async Task RunAsync_WithQuestionOverLimit_ReturnsBadRequest()
    {
        var question = new string('a', ChatFunction.MaximumQuestionLength + 1);
        var function = CreateFunction(new FakeChatResponder("unused"));
        var request = CreateRequest($$"""{"question":"{{question}}"}""");

        var result = await function.RunAsync(request, CancellationToken.None);

        AssertError(
            result,
            $"The question field must not exceed {ChatFunction.MaximumQuestionLength} characters.");
    }

    private static ChatFunction CreateFunction(IChatResponder responder)
    {
        var options = Options.Create(new FoundryOptions
        {
            Endpoint = "https://example.openai.azure.com/",
            DeploymentName = DeploymentName
        });

        return new ChatFunction(responder, options, NullLogger<ChatFunction>.Instance);
    }

    private static HttpRequest CreateRequest(string body)
    {
        var context = new DefaultHttpContext();
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        context.Request.ContentType = "application/json";
        return context.Request;
    }

    private static void AssertError(IActionResult result, string expectedMessage)
    {
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var error = Assert.IsType<ErrorResponse>(badRequest.Value);
        Assert.Equal(expectedMessage, error.Error);
    }

    private sealed class FakeChatResponder(string answer) : IChatResponder
    {
        public string? LastQuestion { get; private set; }

        public Task<string> GetAnswerAsync(string question, CancellationToken cancellationToken)
        {
            LastQuestion = question;
            return Task.FromResult(answer);
        }
    }
}
