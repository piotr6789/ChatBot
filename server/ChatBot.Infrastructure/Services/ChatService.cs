using ChatBot.Common.DTOs;
using ChatBot.Domain.Enums;
using ChatBot.Persistence.Entities;
using ChatBot.Persistence.Repositories;

namespace ChatBot.Infrastructure.Services
{
    public class ChatService : IChatService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IMessageRepository _messageRepository;

        public ChatService(ISessionRepository sessionRepository, IMessageRepository messageRepository)
        {
            _sessionRepository = sessionRepository;
            _messageRepository = messageRepository;
        }

        public async Task<SessionDto> GetChatHistoryAsync(string cookieId)
        {
            var session = await _sessionRepository.GetSessionByCookieIdAsync(cookieId);

            if (session == null) return null;

            return new SessionDto
            {
                Id = session.Id,
                CookieId = session.CookieId,
                CreatedAt = session.CreatedAt,
                LastActivityAt = session.LastActivityAt,
                Messages = session.Messages.Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    Sender = m.Sender.ToString(),
                    CreatedAt = m.CreatedAt,
                    Rating = m.Rating?.ToString()
                }).ToList()
            };
        }

        public async Task<MessageDto> SendMessageAsync(string content, string cookieId)
        {
            var session = await _sessionRepository.GetSessionByCookieIdAsync(cookieId);

            if (session == null)
            {
                session = new Session
                {
                    CookieId = cookieId,
                    CreatedAt = DateTime.UtcNow,
                    LastActivityAt = DateTime.UtcNow
                };
                await _sessionRepository.CreateSessionAsync(session);
            }

            var message = new Message
            {
                Session = session,
                Sender = SenderType.User,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddMessageAsync(message);
            await _messageRepository.SaveChangesAsync();

            return new MessageDto
            {
                Id = message.Id,
                Content = message.Content,
                Sender = message.Sender.ToString(),
                CreatedAt = message.CreatedAt
            };
        }

        public async Task RateMessageAsync(int messageId, MessageRating rating)
        {
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            if (message == null)
                throw new Exception($"Messaig with ID: {messageId} does not exist.");

            message.Rating = rating;

            await _messageRepository.SaveChangesAsync();
        }
    }
}
