using OllamaSharp;

public class OllamaGameActionProvider : GameActionProviderBase, IGameActionProvider
{
    public OllamaGameActionProvider(string model = "phi4-mini", string uriString = "http://localhost:11434")
    {
        Uri uri = new(uriString);
        var ollama = new OllamaApiClient(uri);
        ollama.SelectedModel = model;

        chat = new OllamaApiClient(uri, model);
    }
}
