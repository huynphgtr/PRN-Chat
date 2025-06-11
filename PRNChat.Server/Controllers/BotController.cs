using Microsoft.AspNetCore.Mvc;
using PRNChat.Shared.DTOs;

namespace PRNChat.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BotController : ControllerBase
{
    private readonly ILogger<BotController> _logger;

    public BotController(ILogger<BotController> logger)
    {
        _logger = logger;
    }

    [HttpPost("chat")]
    public Task<ActionResult<BotResponseDTO>> ChatWithBot([FromBody] BotRequestDTO request)
    {
        try
        {
            // Placeholder implementation - integrate with actual AI services
            var response = new BotResponseDTO
            {
                Response = $"Bot response to: {request.Message}",
                Success = true,
                Data = new Dictionary<string, object>
                {
                    { "timestamp", DateTime.UtcNow },
                    { "botType", request.BotType.ToString() }
                }
            };

            return Task.FromResult<ActionResult<BotResponseDTO>>(Ok(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing bot request");
            return Task.FromResult<ActionResult<BotResponseDTO>>(StatusCode(500, new BotResponseDTO
            {
                Success = false,
                ErrorMessage = "Internal server error",
                Response = string.Empty
            }));
        }
    }

    [HttpGet("types")]
    public ActionResult<IEnumerable<string>> GetBotTypes()
    {
        try
        {
            var botTypes = Enum.GetNames<BotType>();
            return Ok(botTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bot types");
            return StatusCode(500, "Internal server error");
        }
    }
}

