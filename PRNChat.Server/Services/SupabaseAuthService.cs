using Microsoft.Extensions.Logging;
using PRNChat.Server.Models;
using PRNChat.Shared.DTOs;
using PRNChat.Shared.Interfaces;
using Supabase;
using Supabase.Gotrue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Client = Supabase.Client;

namespace PRNChat.Server.Services
{
    public class SupabaseAuthService : IAuthService
    {
        private readonly Client _supabaseClient;
        private readonly ILogger<SupabaseAuthService> _logger;

        public SupabaseAuthService(Client supabaseClient, ILogger<SupabaseAuthService> logger)
        {
            _supabaseClient = supabaseClient;
            _logger = logger;
        }

        public async Task<AuthResult> LoginAsync(string email, string password)
        {
            try
            {
                var session = await _supabaseClient.Auth.SignIn(email, password);
                if (session?.User?.Id == null)
                    return new AuthResult { Success = false, ErrorMessage = "Invalid credentials" };

                // Lấy thông tin profile từ bảng 'profiles'
                var userProfile = await GetUserByIdAsync(session.User.Id);
                if (userProfile == null)
                    return new AuthResult { Success = false, ErrorMessage = "User profile not found" };

                return new AuthResult
                {
                    Success = true,
                    User = userProfile,
                    Token = session.AccessToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", email);
                return new AuthResult { Success = false, ErrorMessage = "Login failed" };
            }
        }

        public async Task<AuthResult> RegisterAsync(string email, string password, string username)
        {
            try
            {
                var session = await _supabaseClient.Auth.SignUp(email, password, new SignUpOptions
                {
                    Data = new Dictionary<string, object>
                    {
                        { "display_name", username },
                        { "full_name", username }, // Khởi tạo full_name
                        { "avatar_url", $"https://api.dicebear.com/8.x/initials/svg?seed={username}" } // Avatar mặc định
                    }
                });

                if (session?.User?.Id == null)
                    return new AuthResult { Success = false, ErrorMessage = "Registration failed" };

                // Supabase đã tự động tạo profile qua trigger, chỉ cần lấy lại thông tin
                var userProfile = await GetUserByIdAsync(session.User.Id);
                if (userProfile == null)
                    return new AuthResult { Success = false, ErrorMessage = "Failed to create user profile" };

                return new AuthResult
                {
                    Success = true,
                    User = userProfile,
                    Token = session.AccessToken
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", email);
                return new AuthResult { Success = false, ErrorMessage = "Registration failed" };
            }
        }


        
        public async Task<UserDTO?> GetUserByIdAsync(string userId)
        {
            try
            {
                var response = await _supabaseClient.From<User>()
                    .Where(u => u.Id == userId)
                    .Single();

                if (response == null) return null;

                return new UserDTO
                {
                    Id = response.Id,
                    Email = response.Email,
                    Username = response.DisplayName, // Map DisplayName to Username for compatibility
                    DisplayName = response.DisplayName,
                    FullName = response.FullName,
                    AvatarUrl = response.AvatarUrl
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {UserId}", userId);
                return null;
            }
        }

        public async Task<IEnumerable<UserDTO>> SearchUsersAsync(string query)
        {
            try
            {
                var response = await _supabaseClient.From<User>()
                    .Filter("display_name", Supabase.Postgrest.Constants.Operator.ILike, $"%{query}%")
                    .Limit(10)
                    .Get();

                return response.Models?.Select(u => new UserDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    Username = u.DisplayName, // Map DisplayName to Username for compatibility
                    DisplayName = u.DisplayName,
                    FullName = u.FullName,
                    AvatarUrl = u.AvatarUrl
                }) ?? Enumerable.Empty<UserDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users with query: {Query}", query);
                return Enumerable.Empty<UserDTO>();
            }
        }

        public async Task<bool> UpdateUserProfileAsync(UserDTO userDto)
        {
            try
            {
                var user = new User
                {
                    Id = userDto.Id,
                    DisplayName = userDto.DisplayName,
                    FullName = userDto.FullName,
                    AvatarUrl = userDto.AvatarUrl
                };

                await _supabaseClient.From<User>().Update(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user ID: {UserId}", userDto.Id);
                return false;
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                await _supabaseClient.Auth.SignOut();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return false;
            }
        }

        public async Task<UserDTO?> GetCurrentUserAsync()
        {
            try
            {
                var user = _supabaseClient.Auth.CurrentUser;
                if (user?.Id == null) return null;

                return await GetUserByIdAsync(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return null;
            }
        }

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            try
            {
                await _supabaseClient.Auth.Update(new UserAttributes { Password = newPassword });
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string email)
        {
            try
            {
                await _supabaseClient.Auth.ResetPasswordForEmail(email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for email: {Email}", email);
                return false;
            }
        }
    }
}