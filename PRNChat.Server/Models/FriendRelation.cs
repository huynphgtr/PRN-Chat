using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PRNChat.Server.Models;

[Table("friend_relations")]
public class FriendRelation : BaseModel
{
    [PrimaryKey("id")]
    public long Id { get; set; }
    
    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;
    
    [Column("friend_id")]
    public string FriendId { get; set; } = string.Empty;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
