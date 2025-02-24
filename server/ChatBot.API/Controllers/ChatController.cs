using Microsoft.AspNetCore.Mvc;
using MediatR;
using ChatBot.Application.Messages.Commands;
using ChatBot.Application.Messages.Queries;
using ChatBot.API.Hubs;
using Microsoft.AspNetCore.SignalR;
using ChatBot.Common.DTOs;

namespace ChatBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<ChatHub> _chatHub;

        public ChatController(IMediator mediator, IHubContext<ChatHub> chatHub)
        {
            _mediator = mediator;
            _chatHub = chatHub;
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetChatHistory()
        {
            var cookieId = Request.Cookies["ChatSessionId"];
            if (string.IsNullOrEmpty(cookieId))
                return BadRequest("Brak cookieId, sesja nieznaleziona.");

            var query = new GetChatHistoryQuery { CookieId = cookieId };
            var session = await _mediator.Send(query);

            return Ok(session);
        }

        [HttpPost("message")]
        public async Task<IActionResult> SendMessage([FromBody] CreateMessageDto dto)
        {
            var cookieId = Request.Cookies["ChatSessionId"];

            if (string.IsNullOrEmpty(cookieId))
            {
                cookieId = Guid.NewGuid().ToString();
                Response.Cookies.Append("ChatSessionId", cookieId);
            }

            var command = new CreateMessageCommand { Content = dto.Content, CookieId = cookieId };
            var message = await _mediator.Send(command);

            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "User", dto.Content);
            await _chatHub.Clients.All.SendAsync("SendMessage", cookieId, dto.Content);

            return Ok(message);
        }

        [HttpPut("message/{messageId}/rate")]
        public async Task<IActionResult> RateMessage(int messageId, [FromBody] RateMessageDto dto)
        {
            var command = new RateMessageCommand
            {
                MessageId = messageId,
                Rating = dto.Rating
            };
            await _mediator.Send(command);

            return NoContent();
        }

        [HttpPut("message/cancel")]
        public async Task<IActionResult> CancelMessage()
        {
            await _chatHub.Clients.All.SendAsync("CancelMessage");
            return NoContent();
        }
    }
}
