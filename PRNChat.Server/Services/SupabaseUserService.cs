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
    public class SupabaseUserService : IUserService
    {
        private readonly Client _supabaseClient;
        private readonly ILogger<SupabaseUserService> _logger;

        public SupabaseUserService(Client supabaseClient, ILogger<SupabaseUserService> logger)
        {
            _supabaseClient = supabaseClient;
            _logger = logger;
        }

        public async Task<IEnumerable<UserDTO>> GetContactsAsync(string userId)
        {
            try
            {
                // Lấy ID của tất cả bạn bè
                var relations = await _supabaseClient.From<FriendRelation>()
                    .Select("friend_id")
                    .Where(r => r.UserId == userId)
                    .Get();

                if (relations.Models == null || !relations.Models.Any())
                    return Enumerable.Empty<UserDTO>();

                var friendIds = relations.Models.Select(r => r.FriendId).ToList();

                // Lấy thông tin profile của bạn bè
                var friends = await _supabaseClient.From<User>()
                    .Filter("id", Supabase.Postgrest.Constants.Operator.In, friendIds)
                    .Get();

                return friends.Models?.Select(u => new UserDTO
                {
                    id = u.id,
                    email = u.email,
                    user_name = u.user_name,                  
                    full_name = u.full_name,
                    avatar_url = u.avatar_url
                }) ?? Enumerable.Empty<UserDTO>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contacts for user {UserId}", userId);
                return Enumerable.Empty<UserDTO>();
            }
        }

        public async Task<bool> SendFriendRequestAsync(string requesterId, string receiverId)
        {
            try
            {
                var request = new FriendRequest
                {
                    RequesterId = requesterId,
                    ReceiverId = receiverId,
                    Status = "pending",
                    CreatedAt = DateTime.UtcNow
                };
                await _supabaseClient.From<FriendRequest>().Insert(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending friend request from {RequesterId} to {ReceiverId}", requesterId, receiverId);
                return false;
            }
        }
        
        public async Task<bool> AcceptFriendRequestAsync(long requestId)
        {
            try
            {
                // Lấy thông tin request
                var request = await _supabaseClient.From<FriendRequest>()
                    .Where(r => r.Id == requestId)
                    .Single();

                if (request == null || request.Status != "pending")
                    return false;

                // Cập nhật trạng thái request
                request.Status = "accepted";
                await _supabaseClient.From<FriendRequest>().Update(request);

                // Tạo mối quan hệ bạn bè 2 chiều
                var relation1 = new FriendRelation
                {
                    UserId = request.RequesterId,
                    FriendId = request.ReceiverId,
                    CreatedAt = DateTime.UtcNow
                };
                var relation2 = new FriendRelation
                {
                    UserId = request.ReceiverId,
                    FriendId = request.RequesterId,
                    CreatedAt = DateTime.UtcNow
                };

                await _supabaseClient.From<FriendRelation>().Insert(new[] { relation1, relation2 });
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting friend request {RequestId}", requestId);
                return false;
            }
        }

        public async Task<bool> RemoveFriendAsync(string userId, string friendId)
        {
            try
            {
                // Xóa mối quan hệ 2 chiều
                await _supabaseClient.From<FriendRelation>()
                    .Where(r => r.UserId == userId && r.FriendId == friendId)
                    .Delete();

                await _supabaseClient.From<FriendRelation>()
                    .Where(r => r.UserId == friendId && r.FriendId == userId)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing friend {FriendId} for user {UserId}", friendId, userId);
                return false;
            }
        }

        public async Task<bool> DeclineFriendRequestAsync(long requestId)
        {
            try
            {
                var request = await _supabaseClient.From<FriendRequest>()
                    .Where(r => r.Id == requestId)
                    .Single();

                if (request == null || request.Status != "pending")
                    return false;

                request.Status = "declined";
                await _supabaseClient.From<FriendRequest>().Update(request);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error declining friend request {RequestId}", requestId);
                return false;
            }
        }

        public async Task<IEnumerable<FriendRequestDTO>> GetPendingFriendRequestsAsync(string userId)
        {
            try
            {
                var requests = await _supabaseClient.From<FriendRequest>()
                    .Where(r => r.ReceiverId == userId && r.Status == "pending")
                    .Get();

                if (requests.Models == null || !requests.Models.Any())
                    return Enumerable.Empty<FriendRequestDTO>();

                var result = new List<FriendRequestDTO>();
                foreach (var request in requests.Models)
                {
                    var requesterInfo = await _supabaseClient.From<User>()
                        .Where(u => u.id == request.RequesterId)
                        .Single();

                    result.Add(new FriendRequestDTO
                    {
                        Id = request.Id,
                        RequesterId = request.RequesterId,
                        ReceiverId = request.ReceiverId,
                        Status = request.Status,
                        CreatedAt = request.CreatedAt,
                        RequesterInfo = requesterInfo != null ? new UserDTO
                        {
                            id = requesterInfo.id,
                            email = requesterInfo.email,
                            user_name = requesterInfo.user_name,                            
                            full_name = requesterInfo.full_name,
                            avatar_url = requesterInfo.avatar_url
                        } : null
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending friend requests for user {UserId}", userId);
                return Enumerable.Empty<FriendRequestDTO>();
            }
        }

        public async Task<IEnumerable<FriendRequestDTO>> GetSentFriendRequestsAsync(string userId)
        {
            try
            {
                var requests = await _supabaseClient.From<FriendRequest>()
                    .Where(r => r.RequesterId == userId && r.Status == "pending")
                    .Get();

                if (requests.Models == null || !requests.Models.Any())
                    return Enumerable.Empty<FriendRequestDTO>();

                var result = new List<FriendRequestDTO>();
                foreach (var request in requests.Models)
                {
                    var receiverInfo = await _supabaseClient.From<User>()
                        .Where(u => u.id == request.ReceiverId)
                        .Single();

                    result.Add(new FriendRequestDTO
                    {
                        Id = request.Id,
                        RequesterId = request.RequesterId,
                        ReceiverId = request.ReceiverId,
                        Status = request.Status,
                        CreatedAt = request.CreatedAt,
                        ReceiverInfo = receiverInfo != null ? new UserDTO
                        {
                            id = receiverInfo.id,
                            email = receiverInfo.email,
                            user_name = receiverInfo.user_name,                           
                            full_name = receiverInfo.full_name,
                            avatar_url = receiverInfo.avatar_url
                        } : null
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sent friend requests for user {UserId}", userId);
                return Enumerable.Empty<FriendRequestDTO>();
            }
        }
    }
}