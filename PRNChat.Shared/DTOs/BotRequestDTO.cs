namespace PRNChat.Shared.DTOs;

public class BotRequestDTO
{
    public string Message { get; set; } = string.Empty;
    public string ChatRoomId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public BotType BotType { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
}

public class BotResponseDTO
{
    public string Response { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
}

public enum BotType
{
    ChatGPT,
    Gemini,
    Custom
}

