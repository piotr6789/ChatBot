using MediatR;
using ChatBot.Domain.Enums;

namespace ChatBot.Application.Messages.Commands
{
    public class RateMessageCommand : IRequest
    {
        public int MessageId { get; set; }
        public MessageRating Rating { get; set; }
    }
}
