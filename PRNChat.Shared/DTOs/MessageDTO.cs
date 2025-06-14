namespace PRNChat.Shared.DTOs;

public class MessageDTO
{
    public string Id { get; set; } = string.Empty;
    public string ChatRoomId { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public MessageType Type { get; set; }
    public bool IsRead { get; set; }
}

public enum MessageType
{
    Text,
    Image,
    File,
    System
}

