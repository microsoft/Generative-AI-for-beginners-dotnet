namespace AzFunction_BasicChat_01;

public interface IChatResponder
{
    Task<string> GetAnswerAsync(string question, CancellationToken cancellationToken);
}
