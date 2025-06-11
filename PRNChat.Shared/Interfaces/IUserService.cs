using PRNChat.Shared.DTOs;

namespace PRNChat.Shared.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetContactsAsync(string userId);
    Task<bool> SendFriendRequestAsync(string requesterId, string receiverId);
    Task<bool> AcceptFriendRequestAsync(long requestId);
    Task<bool> DeclineFriendRequestAsync(long requestId);
    Task<bool> RemoveFriendAsync(string userId, string friendId);
    Task<IEnumerable<FriendRequestDTO>> GetPendingFriendRequestsAsync(string userId);
    Task<IEnumerable<FriendRequestDTO>> GetSentFriendRequestsAsync(string userId);
}

public class FriendRequestDTO
{
    public long Id { get; set; }
    public string RequesterId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public UserDTO? RequesterInfo { get; set; }
    public UserDTO? ReceiverInfo { get; set; }
    public string Status { get; set; } = string.Empty; // pending, accepted, declined
    public DateTime CreatedAt { get; set; }
}
