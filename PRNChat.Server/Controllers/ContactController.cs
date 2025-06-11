using Microsoft.AspNetCore.Mvc;
using PRNChat.Shared.DTOs;
using PRNChat.Shared.Interfaces;

namespace PRNChat.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<ContactController> _logger;

    public ContactController(IAuthService authService, ILogger<ContactController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> SearchUsers([FromQuery] string query)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query parameter is required");

            var users = await _authService.SearchUsersAsync(query);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with query: {Query}", query);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<UserDTO>> GetUser(string userId)
    {
        try
        {
            // This would need to be implemented in a user service
            // For now, using auth service as placeholder
            var user = await _authService.GetCurrentUserAsync();
            if (user == null || user.Id != userId)
                return NotFound();

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", userId);
            return StatusCode(500, "Internal server error");
        }
    }
}

