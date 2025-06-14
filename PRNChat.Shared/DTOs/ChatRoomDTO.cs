namespace PRNChat.Shared.DTOs;

public class ChatRoomDTO
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ChatRoomType Type { get; set; }
    public List<UserDTO> Members { get; set; } = new();
    public MessageDTO? LastMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public enum ChatRoomType
{
    Private,
    Group,
    Channel
}

