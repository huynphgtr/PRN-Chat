using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using PRNChat.Client.Config;
using PRNChat.Shared.DTOs;
using PRNChat.Shared.Interfaces;

namespace PRNChat.Client.Services;

public class ApiService : IAuthService, IChatService
{
    private readonly HttpClient _httpClient;
    private readonly AppConfig _config;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(HttpClient httpClient, AppConfig config)
    {
        _httpClient = httpClient;
        _config = config;
        _httpClient.BaseAddress = new Uri(config.ApiBaseUrl);
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    // IAuthService implementation
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            var request = new { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync($"{_config.Endpoints.Auth}/login", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResult>(_jsonOptions);
                return result ?? new AuthResult { Success = false, ErrorMessage = "Invalid response" };
            }
            
            return new AuthResult { Success = false, ErrorMessage = "Login failed" };
        }
        catch (Exception ex)
        {
            return new AuthResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string username)
    {
        try
        {
            var request = new { Email = email, Password = password, Username = username };
            var response = await _httpClient.PostAsJsonAsync($"{_config.Endpoints.Auth}/register", request);
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResult>(_jsonOptions);
                return result ?? new AuthResult { Success = false, ErrorMessage = "Invalid response" };
            }
            
            return new AuthResult { Success = false, ErrorMessage = "Registration failed" };
        }
        catch (Exception ex)
        {
            return new AuthResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<bool> LogoutAsync()
    {
        try
        {
            var response = await _httpClient.PostAsync($"{_config.Endpoints.Auth}/logout", null);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<UserDTO?> GetCurrentUserAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_config.Endpoints.Auth}/profile");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDTO>(_jsonOptions);
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateUserProfileAsync(UserDTO user)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_config.Endpoints.Auth}/profile", user);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            var request = new { CurrentPassword = currentPassword, NewPassword = newPassword };
            var response = await _httpClient.PostAsJsonAsync($"{_config.Endpoints.Auth}/change-password", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(string email)
    {
        try
        {
            var request = new { Email = email };
            var response = await _httpClient.PostAsJsonAsync($"{_config.Endpoints.Auth}/reset-password", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<UserDTO>> SearchUsersAsync(string query)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_config.Endpoints.Contact}/search?query={Uri.EscapeDataString(query)}");
            
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>(_jsonOptions);
                return users ?? Enumerable.Empty<UserDTO>();
            }
            
            return Enumerable.Empty<UserDTO>();
        }
        catch
        {
            return Enumerable.Empty<UserDTO>();
        }
    }

    // IChatService implementation
    public async Task<IEnumerable<ChatRoomDTO>> GetChatRoomsAsync(string userId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_config.Endpoints.Chat}/rooms/{userId}");
            
            if (response.IsSuccessStatusCode)
            {
                var chatRooms = await response.Content.ReadFromJsonAsync<IEnumerable<ChatRoomDTO>>(_jsonOptions);
                return chatRooms ?? Enumerable.Empty<ChatRoomDTO>();
            }
            
            return Enumerable.Empty<ChatRoomDTO>();
        }
        catch
        {
            return Enumerable.Empty<ChatRoomDTO>();
        }
    }

    public async Task<ChatRoomDTO?> GetChatRoomAsync(string chatRoomId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_config.Endpoints.Chat}/rooms/details/{chatRoomId}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ChatRoomDTO>(_jsonOptions);
            }
            
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<ChatRoomDTO?> CreateChatRoomAsync(ChatRoomDTO chatRoom)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_config.Endpoints.Chat}/rooms", chatRoom);
            
            if (response.IsSuccessStatusCode)
            {
                var createdRoom = await response.Content.ReadFromJsonAsync<ChatRoomDTO>(_jsonOptions);
                return createdRoom ?? chatRoom;
            }
            
            throw new InvalidOperationException("Failed to create chat room");
        }
        catch
        {
            throw;
        }
    }

    public async Task<ChatRoomDTO?> UpdateChatRoomAsync(ChatRoomDTO chatRoom)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_config.Endpoints.Chat}/rooms/{chatRoom.Id}", chatRoom);

            if (response.IsSuccessStatusCode)
            {
                var updatedRoom = await response.Content.ReadFromJsonAsync<ChatRoomDTO>(_jsonOptions);
                return updatedRoom ?? chatRoom;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteChatRoomAsync(string chatRoomId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_config.Endpoints.Chat}/rooms/{chatRoomId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<MessageDTO>> GetMessagesAsync(string chatRoomId, int limit = 50, int offset = 0)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_config.Endpoints.Chat}/messages/{chatRoomId}?limit={limit}&offset={offset}");
            
            if (response.IsSuccessStatusCode)
            {
                var messages = await response.Content.ReadFromJsonAsync<IEnumerable<MessageDTO>>(_jsonOptions);
                return messages ?? Enumerable.Empty<MessageDTO>();
            }
            
            return Enumerable.Empty<MessageDTO>();
        }
        catch
        {
            return Enumerable.Empty<MessageDTO>();
        }
    }

    public async Task<MessageDTO?> SendMessageAsync(MessageDTO message)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{_config.Endpoints.Chat}/messages", message);
            
            if (response.IsSuccessStatusCode)
            {
                var sentMessage = await response.Content.ReadFromJsonAsync<MessageDTO>(_jsonOptions);
                return sentMessage ?? message;
            }
            
            throw new InvalidOperationException("Failed to send message");
        }
        catch
        {
            throw;
        }
    }

    public async Task<bool> DeleteMessageAsync(string messageId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_config.Endpoints.Chat}/messages/{messageId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> MarkMessageAsReadAsync(string messageId, string userId)
    {
        try
        {
            var request = new { MessageId = messageId, UserId = userId };
            var response = await _httpClient.PostAsJsonAsync($"{_config.Endpoints.Chat}/messages/mark-read", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> AddUserToChatRoomAsync(string chatRoomId, string userId)
    {
        try
        {
            var request = new { ChatRoomId = chatRoomId, UserId = userId };
            var response = await _httpClient.PostAsJsonAsync($"{_config.Endpoints.Chat}/rooms/add-user", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveUserFromChatRoomAsync(string chatRoomId, string userId)
    {
        try
        {
            var request = new { ChatRoomId = chatRoomId, UserId = userId };
            var response = await _httpClient.PostAsJsonAsync($"{_config.Endpoints.Chat}/rooms/remove-user", request);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}

