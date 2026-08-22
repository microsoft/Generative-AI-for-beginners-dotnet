using Microsoft.Extensions.AI;

namespace AzFunction_BasicChat_01;

public sealed class FoundryChatResponder(IChatClient chatClient) : IChatResponder
{
    public async Task<string> GetAnswerAsync(string question, CancellationToken cancellationToken)
    {
        var response = await chatClient.GetResponseAsync(question, cancellationToken: cancellationToken);
        return response.Text;
    }
}
