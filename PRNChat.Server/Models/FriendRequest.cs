using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace PRNChat.Server.Models;

[Table("friend_requests")]
public class FriendRequest : BaseModel
{
    [PrimaryKey("id")]
    public long Id { get; set; }
    
    [Column("requester_id")]
    public string RequesterId { get; set; } = string.Empty;
    
    [Column("receiver_id")]
    public string ReceiverId { get; set; } = string.Empty;
    
    [Column("status")]
    public string Status { get; set; } = string.Empty; // pending, accepted, declined
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
