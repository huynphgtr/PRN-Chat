namespace PRNChat.Server.Config;

public class SupabaseConfig
{
    public string Url { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public string ServiceKey { get; set; } = string.Empty;
    public SupabaseOptions Options { get; set; } = new();
}

public class SupabaseOptions
{
    public bool AutoConnectRealtime { get; set; } = true;
    public bool AutoRefreshToken { get; set; } = true;
    public bool PersistSession { get; set; } = true;
    public string Schema { get; set; } = "public";
    public int SessionExpiryMargin { get; set; } = 60;
}

