using PRNChat.Shared.DTOs;

namespace PRNChat.Shared.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RegisterAsync(string email, string password, string user_name, string full_name);
    Task<bool> LogoutAsync();
    Task<UserDTO?> GetCurrentUserAsync();
    Task<bool> UpdateUserProfileAsync(UserDTO user);
    Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(string email);
    Task<IEnumerable<UserDTO>> SearchUsersAsync(string query);
}

public class AuthResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public UserDTO? User { get; set; }
    public string? Token { get; set; }
}

