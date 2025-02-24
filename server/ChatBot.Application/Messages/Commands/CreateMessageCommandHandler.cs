using MediatR;
using ChatBot.Common.DTOs;
using ChatBot.Infrastructure.Services;

namespace ChatBot.Application.Messages.Commands
{
    public class CreateMessageCommandHandler : IRequestHandler<CreateMessageCommand, MessageDto>
    {
        private readonly IChatService _chatService;

        public CreateMessageCommandHandler(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<MessageDto> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            return await _chatService.SendMessageAsync(request.Content, request.CookieId);
        }
    }
}
