// PRNChat.Server/Models/RoomParticipant.cs
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("room_participants")]
public class RoomParticipant : BaseModel
{
    [PrimaryKey("id")]
    public long Id { get; set; }

    [Column("room_id")]
    public string RoomId { get; set; } = string.Empty;

    [Column("user_id")]
    public string UserId { get; set; } = string.Empty;

    [Column("joined_at")]
    public DateTime JoinedAt { get; set; }
}