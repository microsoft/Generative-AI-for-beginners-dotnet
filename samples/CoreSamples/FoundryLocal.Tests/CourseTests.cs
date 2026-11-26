using FoundryLocal.Course;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Xunit;

public sealed class CourseTests
{
    [Theory]
    [InlineData("The library opens at 9 AM.", true)]
    [InlineData("9:00 a.m.", true)]
    [InlineData("19 AM", false)]
    [InlineData("9 PM", false)]
    [InlineData("9", false)]
    public void OpeningTimeRequiresTheActualMorningHour(string text, bool expected) =>
        Assert.Equal(expected, FoundryCourseChecks.ContainsOpeningTime(text));

    [Theory]
    [InlineData("""{"Sentiment":"Positive","Rating":5}""")]
    [InlineData("""{ "Rating": 5, "Sentiment": "Positive" }""")]
    public void ReviewValidatesTypedContract(string json) =>
        Assert.Equal(5, FoundryCourseChecks.ParseReview(json).Rating);

    [Theory]
    [InlineData("""{"Sentiment":"Positive"}""")]
    [InlineData("""{"Sentiment":"Positive","Rating":"5"}""")]
    [InlineData("""{"Sentiment":"Positive","Rating":5,"extra":true}""")]
    [InlineData("not JSON")]
    public void MalformedReviewIsAnError(string json) =>
        Assert.Throws<JsonException>(() => FoundryCourseChecks.ParseReview(json));

    [Theory]
    [InlineData("""{"Sentiment":"Negative","Rating":5}""")]
    [InlineData("""{"Sentiment":"Positive","Rating":1}""")]
    [InlineData("null")]
    public void WrongReviewSemanticsIsAnError(string json) =>
        Assert.Throws<InvalidOperationException>(() => FoundryCourseChecks.ParseReview(json));

    [Theory]
    [InlineData("")]
    [InlineData(" \n")]
    [InlineData(null)]
    public void EmptyTextFails(string? text) =>
        Assert.Throws<InvalidOperationException>(() => FoundryCourseChecks.RequireText(text));

    [Fact]
    public void ModeRejectsUnknownAndDuplicateArguments()
    {
        Assert.Equal("hello", FoundryCourseChecks.Mode(["--prepare"], "hello", "hello"));
        Assert.Throws<ArgumentException>(() => FoundryCourseChecks.Mode(["--prepare", "--prepare"], "hello", "hello"));
        Assert.Throws<ArgumentException>(() => FoundryCourseChecks.Mode(["bad"], "hello", "hello"));
        Assert.Throws<ArgumentException>(() => FoundryCourseChecks.Mode(["hello", "hello"], "hello", "hello"));
    }

    [Fact]
    public void ModelTasksAndToolFlagsAreStrict()
    {
        FoundryCourseRuntime.ValidateModel("chat-completion", true, false, true);
        FoundryCourseRuntime.ValidateModel("embeddings", false, true, false);
        Assert.Throws<InvalidOperationException>(() => FoundryCourseRuntime.ValidateModel("embeddings", true, false, false));
        Assert.Throws<InvalidOperationException>(() => FoundryCourseRuntime.ValidateModel("chat-completion", false, true, false));
        Assert.Throws<InvalidOperationException>(() => FoundryCourseRuntime.ValidateModel("chat-completion", null, false, true));
    }

    [Fact]
    public void MissingCacheRequiresExplicitPreparation()
    {
        FoundryCourseRuntime.ValidateCache(true, false, "cached");
        FoundryCourseRuntime.ValidateCache(false, true, "download-approved");
        var error = Assert.Throws<InvalidOperationException>(() =>
            FoundryCourseRuntime.ValidateCache(false, false, "missing"));
        Assert.Contains("--prepare", error.Message);
    }

    [Fact]
    public async Task CachedResponseAvoidsSecondModelInvocation()
    {
        using var fake = new RepeatingClient();
        var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
        using var client = fake.AsBuilder().UseDistributedCache(cache).Build();
        var options = new ChatOptions { ToolMode = ChatToolMode.None };
        var first = await client.GetResponseAsync("repeat", options);
        var second = await client.GetResponseAsync("repeat", options);
        Assert.Equal(first.Text, second.Text);
        Assert.Equal(1, fake.Requests);
    }

    [Fact]
    public async Task MalformedToolArgumentsCannotInvokeTheFunction()
    {
        int calls = 0;
        var tool = AIFunctionFactory.Create((int count) => { calls++; return count; }, "Count");
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await tool.InvokeAsync(new AIFunctionArguments(new Dictionary<string, object?> { ["count"] = "not-a-number" })));
        Assert.Equal(0, calls);
    }

    [Fact]
    public void VectorsRejectMismatchesNonfiniteAndZero()
    {
        Assert.Throws<InvalidOperationException>(() => FoundryCourseChecks.ValidateVector(new float[] { 1, 2 }, 3));
        Assert.Throws<InvalidOperationException>(() => FoundryCourseChecks.ValidateVector(new float[] { float.NaN }, 1));
        Assert.Throws<InvalidOperationException>(() => FoundryCourseChecks.ValidateVector(new float[] { float.PositiveInfinity }, 1));
        Assert.Throws<InvalidOperationException>(() => FoundryCourseChecks.ValidateVector(new float[] { 0 }, 1));
        Assert.Throws<InvalidOperationException>(() => FoundryCourseChecks.ValidateVector(Array.Empty<float>(), 0));
    }

    [Fact]
    public void CosineRanksRelatedVectorsAndRejectsMismatches()
    {
        Assert.Equal(1, FoundryCourseChecks.Cosine(new float[] { 1, 0 }, new float[] { 2, 0 }), 8);
        Assert.Equal(0, FoundryCourseChecks.Cosine(new float[] { 1, 0 }, new float[] { 0, 1 }), 8);
        Assert.Throws<InvalidOperationException>(() => FoundryCourseChecks.Cosine(new float[] { 1, 0 }, new float[] { 1 }));
    }

    [Fact]
    public async Task RepeatedToolRequestsAreBounded()
    {
        int calls = 0;
        using var fake = new RepeatingClient();
        using var client = fake.AsBuilder().UseFunctionInvocation(configure: c => c.MaximumIterationsPerRequest = 3).Build();
        var result = await client.GetResponseAsync("hours", new ChatOptions
        {
            Tools = [AIFunctionFactory.Create(() => { calls++; return "9 AM"; }, "GetLibraryHours")],
            ToolMode = ChatToolMode.RequireAny
        });
        Assert.InRange(calls, 1, 3);
        Assert.True(fake.SawFinalToolFreeRequest);
        Assert.Equal("Stopped.", result.Text);
    }

    [Fact]
    public async Task CancellationIsNotConvertedToSuccess()
    {
        using var fake = new RepeatingClient();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => fake.GetResponseAsync([], cancellationToken: cts.Token));
    }

    private sealed class RepeatingClient : IChatClient
    {
        public bool SawFinalToolFreeRequest { get; private set; }
        public int Requests { get; private set; }

        public Task<ChatResponse> GetResponseAsync(
            IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Requests++;
            if (options?.ToolMode is NoneChatToolMode || options?.Tools is not { Count: > 0 })
            {
                SawFinalToolFreeRequest = true;
                return Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant, "Stopped.")));
            }
            return Task.FromResult(new ChatResponse(new ChatMessage(ChatRole.Assistant,
                [new FunctionCallContent(Guid.NewGuid().ToString(), "GetLibraryHours", new Dictionary<string, object?>())])));
        }

        public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
            IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public object? GetService(Type serviceType, object? serviceKey = null) =>
            serviceKey is null && serviceType.IsInstanceOfType(this) ? this : null;

        public void Dispose() { }
    }
}
