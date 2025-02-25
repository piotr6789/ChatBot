using MediatR;
using ChatBot.Common.DTOs;
using ChatBot.Infrastructure.Services;

namespace ChatBot.Application.Messages.Queries
{
    public class GetChatHistoryQueryHandler : IRequestHandler<GetChatHistoryQuery, SessionDto>
    {
        private readonly IChatService _chatService;

        public GetChatHistoryQueryHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<SessionDto> Handle(GetChatHistoryQuery request, CancellationToken cancellationToken)
        {
            return await _chatService.GetChatHistoryAsync(request.ClientSessionId);
        }
    }
}
