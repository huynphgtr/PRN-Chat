// PRNChat.Server/Models/User.cs
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("profiles")] // <-- Sửa tên bảng thành "profiles"
public class User : BaseModel
{
    [PrimaryKey("id")]
    public string id { get; set; } = string.Empty;

    [Column("email")]
    public string email { get; set; } = string.Empty;
    
    [Column("user_name")] 
    public string user_name { get; set; } = string.Empty;

    [Column("full_name")] 
    public string full_name { get; set; } = string.Empty;
    
    [Column("avatar_url")]
    public string avatar_url { get; set; } = string.Empty;
}