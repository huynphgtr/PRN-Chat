using PRNChat.Shared.DTOs;

namespace PRNChat.Shared.Interfaces;

public interface IChatService
{
    Task<IEnumerable<ChatRoomDTO>> GetChatRoomsAsync(string userId);
    Task<ChatRoomDTO?> GetChatRoomAsync(string chatRoomId);
    Task<ChatRoomDTO?> CreateChatRoomAsync(ChatRoomDTO chatRoom);
    Task<ChatRoomDTO?> UpdateChatRoomAsync(ChatRoomDTO chatRoom);
    Task<bool> DeleteChatRoomAsync(string chatRoomId);
    Task<IEnumerable<MessageDTO>> GetMessagesAsync(string chatRoomId, int limit = 50, int offset = 0);
    Task<MessageDTO?> SendMessageAsync(MessageDTO message);
    Task<bool> DeleteMessageAsync(string messageId);
    Task<bool> MarkMessageAsReadAsync(string messageId, string userId);
    Task<bool> AddUserToChatRoomAsync(string chatRoomId, string userId);
    Task<bool> RemoveUserFromChatRoomAsync(string chatRoomId, string userId);
}

