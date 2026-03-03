namespace MAF_AIWebChatApp_AG_UI.Web.Services;

public class AgentsService
{
    HttpClient httpClient;
    private readonly ILogger<AgentsService> _logger;

    public HttpClient HttpClient { get => httpClient; set => httpClient = value; }

    public AgentsService(HttpClient httpClient, ILogger<AgentsService> logger)
    {
        _logger = logger;
        this.httpClient = httpClient;
    }
}