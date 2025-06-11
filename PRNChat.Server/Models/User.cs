// PRNChat.Server/Models/User.cs
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

[Table("profiles")] // <-- Sửa tên bảng thành "profiles"
public class User : BaseModel
{
    [PrimaryKey("id")]
    public string Id { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;
    
    [Column("display_name")] // <-- Đổi username thành display_name
    public string DisplayName { get; set; } = string.Empty;

    [Column("full_name")] // <-- Thêm full_name
    public string FullName { get; set; } = string.Empty;
    
    [Column("avatar_url")]
    public string AvatarUrl { get; set; } = string.Empty;
}