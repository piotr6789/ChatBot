using ChatBot.Common.DTOs;
using MediatR;

namespace ChatBot.Application.Messages.Queries
{
    public class GetChatHistoryQuery : IRequest<SessionDto>
    {
        public string CookieId { get; set; }
    }
}
