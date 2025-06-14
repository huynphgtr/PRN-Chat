namespace PRNChat.Client.Config;

public class AppConfig
{
    public string ApiBaseUrl { get; set; } = "https://localhost:7000";
    public int RequestTimeoutSeconds { get; set; } = 30;
    public bool EnableLogging { get; set; } = true;
    public string LogLevel { get; set; } = "Information";
    
    public ApiEndpoints Endpoints { get; set; } = new();
}

public class ApiEndpoints
{
    public string Auth { get; set; } = "/api/auth";
    public string Chat { get; set; } = "/api/chat";
    public string Contact { get; set; } = "/api/contact";
    public string Bot { get; set; } = "/api/bot";
}

