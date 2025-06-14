using Microsoft.Extensions.Logging;
using PRNChat.Server.Models;
using PRNChat.Shared.DTOs;
using PRNChat.Shared.Interfaces;
using Supabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRNChat.Server.Services
{
    public class SupabaseChatService : IChatService
    {
        private readonly Client _supabaseClient;
        private readonly ILogger<SupabaseChatService> _logger;

        public SupabaseChatService(Client supabaseClient, ILogger<SupabaseChatService> logger)
        {
            _supabaseClient = supabaseClient;
            _logger = logger;
        }

        public async Task<ChatRoomDTO?> CreateChatRoomAsync(ChatRoomDTO chatRoomDto)
        {
            try
            {
                var newRoom = new ChatRoom
                {
                    Name = chatRoomDto.Name,
                    IsGroup = chatRoomDto.Type == ChatRoomType.Group,
                    CreatedAt = DateTime.UtcNow
                };

                var roomResponse = await _supabaseClient.From<ChatRoom>().Insert(newRoom);
                var createdRoom = roomResponse.Models.FirstOrDefault();

                if (createdRoom == null) throw new Exception("Failed to create room.");

                // Thêm các thành viên vào phòng
                foreach (var member in chatRoomDto.Members)
                {
                    await AddUserToChatRoomAsync(createdRoom.Id, member.id);
                }

                chatRoomDto.Id = createdRoom.Id;
                return chatRoomDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating chat room.");
                return null;
            }
        }

        public async Task<IEnumerable<ChatRoomDTO>> GetChatRoomsAsync(string userId)
        {
            try
            {
                var participantResponse = await _supabaseClient.From<RoomParticipant>()
                    .Select("room_id")
                    .Where(p => p.UserId == userId)
                    .Get();

                if (participantResponse.Models == null || !participantResponse.Models.Any())
                {
                    return Enumerable.Empty<ChatRoomDTO>();
                }

                var roomIds = participantResponse.Models.Select(p => p.RoomId).ToList();

                var roomsResponse = await _supabaseClient.From<ChatRoom>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.In, roomIds)
                    .Get();
                
                return roomsResponse.Models?.Select(r => new ChatRoomDTO
                {
                    Id = r.Id,
                    Name = r.Name,
                    Type = r.IsGroup ? ChatRoomType.Group : ChatRoomType.Private,
                    CreatedAt = r.CreatedAt
                }) ?? Enumerable.Empty<ChatRoomDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat rooms for user {UserId}", userId);
                return Enumerable.Empty<ChatRoomDTO>();
            }
        }

        public async Task<IEnumerable<MessageDTO>> GetMessagesAsync(string chatRoomId, int limit = 50, int offset = 0)
        {
            try
            {
                var response = await _supabaseClient.From<Message>()
                    .Select("*")
                    .Where(m => m.RoomId == chatRoomId)
                    .Order(m => m.SentAt, Supabase.Postgrest.Constants.Ordering.Descending)
                    .Range(offset, offset + limit - 1)
                    .Get();

                return response.Models?.Select(m => new MessageDTO
                {
                    Id = m.Id,
                    ChatRoomId = m.RoomId,
                    SenderId = m.SenderId,
                    Content = m.Content,
                    Timestamp = m.SentAt,
                    Type = Enum.TryParse<MessageType>(m.MessageType, true, out var type) ? type : MessageType.Text
                }) ?? Enumerable.Empty<MessageDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting messages for chat room {ChatRoomId}", chatRoomId);
                return Enumerable.Empty<MessageDTO>();
            }
        }

        public async Task<MessageDTO?> SendMessageAsync(MessageDTO messageDto)
        {
            try
            {
                var message = new Message
                {
                    RoomId = messageDto.ChatRoomId,
                    SenderId = messageDto.SenderId,
                    Content = messageDto.Content,
                    SentAt = DateTime.UtcNow,
                    MessageType = messageDto.Type.ToString().ToLower(),
                    IsBot = false
                };

                var response = await _supabaseClient.From<Message>().Insert(message);
                var createdMessage = response.Models.FirstOrDefault();

                if (createdMessage != null)
                {
                    messageDto.Id = createdMessage.Id;
                    messageDto.Timestamp = createdMessage.SentAt;
                    return messageDto;
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message");
                return null;
            }
        }

        public async Task<bool> AddUserToChatRoomAsync(string chatRoomId, string userId)
        {
            try
            {
                var participant = new RoomParticipant
                {
                    RoomId = chatRoomId,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow
                };
                await _supabaseClient.From<RoomParticipant>().Insert(participant);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user {UserId} to room {ChatRoomId}", userId, chatRoomId);
                return false;
            }
        }

        public async Task<ChatRoomDTO?> GetChatRoomAsync(string chatRoomId)
        {
            try
            {
                var room = await _supabaseClient.From<ChatRoom>()
                    .Where(r => r.Id == chatRoomId)
                    .Single();

                if (room == null) return null;

                // Get members
                var participants = await _supabaseClient.From<RoomParticipant>()
                    .Where(p => p.RoomId == chatRoomId)
                    .Get();

                var memberIds = participants.Models?.Select(p => p.UserId).ToList() ?? new List<string>();

                // Get member details
                var members = new List<UserDTO>();
                if (memberIds.Any())
                {
                    var users = await _supabaseClient.From<User>()
                        .Filter("id", Supabase.Postgrest.Constants.Operator.In, memberIds)
                        .Get();

                    members = users.Models?.Select(u => new UserDTO
                    {
                        id = u.id,
                        email = u.email,
                        user_name = u.user_name,                  
                        full_name = u.full_name,
                        avatar_url = u.avatar_url
                    }).ToList() ?? new List<UserDTO>();
                }

                return new ChatRoomDTO
                {
                    Id = room.Id,
                    Name = room.Name,
                    Type = room.IsGroup ? ChatRoomType.Group : ChatRoomType.Private,
                    CreatedAt = room.CreatedAt,
                    Members = members
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat room {ChatRoomId}", chatRoomId);
                return null;
            }
        }

        public async Task<ChatRoomDTO?> UpdateChatRoomAsync(ChatRoomDTO chatRoomDto)
        {
            try
            {
                var room = new ChatRoom
                {
                    Id = chatRoomDto.Id,
                    Name = chatRoomDto.Name,
                    IsGroup = chatRoomDto.Type == ChatRoomType.Group
                };

                await _supabaseClient.From<ChatRoom>().Update(room);
                return chatRoomDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating chat room {ChatRoomId}", chatRoomDto.Id);
                return null;
            }
        }

        public async Task<bool> DeleteChatRoomAsync(string chatRoomId)
        {
            try
            {
                // First remove all participants
                await _supabaseClient.From<RoomParticipant>()
                    .Where(p => p.RoomId == chatRoomId)
                    .Delete();

                // Then delete the room
                await _supabaseClient.From<ChatRoom>()
                    .Where(r => r.Id == chatRoomId)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting chat room {ChatRoomId}", chatRoomId);
                return false;
            }
        }

        public async Task<bool> RemoveUserFromChatRoomAsync(string chatRoomId, string userId)
        {
            try
            {
                await _supabaseClient.From<RoomParticipant>()
                    .Where(p => p.RoomId == chatRoomId && p.UserId == userId)
                    .Delete();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user {UserId} from room {ChatRoomId}", userId, chatRoomId);
                return false;
            }
        }

        public async Task<bool> DeleteMessageAsync(string messageId)
        {
            try
            {
                await _supabaseClient.From<Message>()
                    .Where(m => m.Id == messageId)
                    .Delete();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message {MessageId}", messageId);
                return false;
            }
        }

        public Task<bool> MarkMessageAsReadAsync(string messageId, string userId)
        {
            try
            {
                // This would require a separate table for read receipts
                // For now, just return true as a placeholder
                _logger.LogInformation("Message {MessageId} marked as read by user {UserId}", messageId, userId);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking message {MessageId} as read for user {UserId}", messageId, userId);
                return Task.FromResult(false);
            }
        }
    }
}