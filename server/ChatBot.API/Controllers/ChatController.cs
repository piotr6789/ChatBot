using Microsoft.AspNetCore.Mvc;
using MediatR;
using ChatBot.Application.Messages.Commands;
using ChatBot.Application.Messages.Queries;
using ChatBot.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using ChatBot.Common.DTOs;
using ChatBot.Infrastructure.Services;

namespace ChatBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            IMediator mediator,
            IHubContext<ChatHub> chatHub,
            IChatService chatService,
            ILogger<ChatController> logger)
        {
            _mediator = mediator;
            _chatHub = chatHub;
            _chatService = chatService;
            _logger = logger;
        }

        [HttpGet("history")]
        public async Task<ActionResult<SessionDto>> GetChatHistory([FromQuery] Guid clientSessionId)
        {
            try
            {
                if (clientSessionId == Guid.Empty)
                {
                    _logger.LogInformation("No session ID provided. Creating a new session.");
                    clientSessionId = Guid.NewGuid();
                    await _chatService.CreateSessionAsync(clientSessionId);
                }

                var query = new GetChatHistoryQuery { ClientSessionId = clientSessionId };
                var sessionData = await _mediator.Send(query);

                if (sessionData == null)
                {
                    _logger.LogWarning("No chat history found for session ID: {ClientSessionId}", clientSessionId);
                    return NotFound($"Chat history not found for session ID: {clientSessionId}");
                }

                return Ok(sessionData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving chat history for session ID: {ClientSessionId}", clientSessionId);
                return StatusCode(500, "An internal error occurred while retrieving chat history.");
            }
        }

        [HttpPut("message/{messageId}/rate")]
        public async Task<IActionResult> RateMessage(int messageId, [FromBody] RateMessageDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Invalid request. RateMessageDto is null for message ID: {MessageId}", messageId);
                    return BadRequest("Invalid request data.");
                }

                var command = new RateMessageCommand
                {
                    MessageId = messageId,
                    Rating = dto.Rating
                };

                await _mediator.Send(command);
                _logger.LogInformation("Successfully rated message ID: {MessageId}", messageId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rating message ID: {MessageId}", messageId);
                return StatusCode(500, "An internal error occurred while rating the message.");
            }
        }

        [HttpPut("message/cancel")]
        public async Task<IActionResult> CancelMessage()
        {
            try
            {
                _logger.LogInformation("Sending cancel message event to all clients.");
                await _chatHub.Clients.All.SendAsync("CancelMessage");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending cancel message event.");
                return StatusCode(500, "An internal error occurred while canceling the message.");
            }
        }
    }
}
