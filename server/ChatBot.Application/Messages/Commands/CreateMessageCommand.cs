using ChatBot.Common.DTOs;
using MediatR;

namespace ChatBot.Application.Messages.Commands
{
    public class CreateMessageCommand : IRequest<MessageDto>
    {
        public string Content { get; set; }
        public string CookieId { get; set; }
    }
}
