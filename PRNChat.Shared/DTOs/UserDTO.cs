namespace PRNChat.Shared.DTOs;

public class UserDTO
{
    public string id { get; set; } = string.Empty; 
    public string full_name { get; set; } = string.Empty;
    public string user_name { get; set; } = string.Empty;
    public string email { get; set; } = string.Empty;
    public string avatar_url { get; set; } = string.Empty;
    public bool is_online { get; set; }
}

