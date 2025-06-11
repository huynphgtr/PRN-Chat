using Microsoft.AspNetCore.Mvc;
using PRNChat.Shared.DTOs;
using PRNChat.Shared.Interfaces;

namespace PRNChat.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatController> _logger;

    public ChatController(IChatService chatService, ILogger<ChatController> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    [HttpGet("rooms/{userId}")]
    public async Task<ActionResult<IEnumerable<ChatRoomDTO>>> GetChatRooms(string userId)
    {
        try
        {
            var chatRooms = await _chatService.GetChatRoomsAsync(userId);
            return Ok(chatRooms);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chat rooms for user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("rooms/details/{chatRoomId}")]
    public async Task<ActionResult<ChatRoomDTO>> GetChatRoom(string chatRoomId)
    {
        try
        {
            var chatRoom = await _chatService.GetChatRoomAsync(chatRoomId);
            if (chatRoom == null)
                return NotFound();
            
            return Ok(chatRoom);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting chat room {ChatRoomId}", chatRoomId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("rooms")]
    public async Task<ActionResult<ChatRoomDTO>> CreateChatRoom([FromBody] ChatRoomDTO chatRoom)
    {
        try
        {
            var createdRoom = await _chatService.CreateChatRoomAsync(chatRoom);
            if (createdRoom == null)
                return StatusCode(500, "Failed to create chat room");

            return CreatedAtAction(nameof(GetChatRoom), new { chatRoomId = createdRoom.Id }, createdRoom);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating chat room");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("messages/{chatRoomId}")]
    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessages(string chatRoomId, [FromQuery] int limit = 50, [FromQuery] int offset = 0)
    {
        try
        {
            var messages = await _chatService.GetMessagesAsync(chatRoomId, limit, offset);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting messages for chat room {ChatRoomId}", chatRoomId);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("messages")]
    public async Task<ActionResult<MessageDTO>> SendMessage([FromBody] MessageDTO message)
    {
        try
        {
            var sentMessage = await _chatService.SendMessageAsync(message);
            return Ok(sentMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("messages/{messageId}")]
    public async Task<ActionResult<bool>> DeleteMessage(string messageId)
    {
        try
        {
            var result = await _chatService.DeleteMessageAsync(messageId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting message {MessageId}", messageId);
            return StatusCode(500, false);
        }
    }
}

