using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PRNChat.Server.Models;

[Table("groups")]
public class Group : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;
    
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    
    [Column("description")]
    public string Description { get; set; } = string.Empty;
    
    [Column("avatar_url")]
    public string AvatarUrl { get; set; } = string.Empty;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
    
    [Column("created_by")]
    public string CreatedBy { get; set; } = string.Empty;
    
    [Column("is_private")]
    public bool IsPrivate { get; set; }
    
    [Column("max_members")]
    public int MaxMembers { get; set; } = 100;
    
    // Navigation properties
    public User? Creator { get; set; }
    public List<User> Members { get; set; } = new();
    public ChatRoom? ChatRoom { get; set; }
}

