using MediatR;
using ChatBot.Infrastructure.Services;

namespace ChatBot.Application.Messages.Commands
{
    public class RateMessageCommandHandler : IRequestHandler<RateMessageCommand>
    {
        private readonly IChatService _chatService;

        public RateMessageCommandHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task Handle(RateMessageCommand request, CancellationToken cancellationToken)
        {
            await _chatService.RateMessageAsync(request.MessageId, request.Rating);
        }
    }
}
