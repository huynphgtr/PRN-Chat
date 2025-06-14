// PRNChat.Server/Models/ChatRoom.cs
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("chat_rooms")]
public class ChatRoom : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;
    
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    
    [Column("is_group")] 
    public bool IsGroup { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}