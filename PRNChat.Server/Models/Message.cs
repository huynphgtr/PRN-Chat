// PRNChat.Server/Models/Message.cs
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("messages")]
public class Message : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;
    
    [Column("room_id")]
    public string RoomId { get; set; } = string.Empty;
    
    [Column("sender_id")]
    public string SenderId { get; set; } = string.Empty;
    
    [Column("content")]
    public string Content { get; set; } = string.Empty;
    
    [Column("message_type")]
    public string MessageType { get; set; } = "text";
    
    [Column("sent_at")]
    public DateTime SentAt { get; set; }
    
    [Column("is_bot")]
    public bool IsBot { get; set; }
}