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
        private readonly IResponseRepository _responseRepository;

        public ChatService(
            ISessionRepository sessionRepository,
            IMessageRepository messageRepository,
            IResponseRepository responseRepository)
        {
            _sessionRepository = sessionRepository;
            _messageRepository = messageRepository;
            _responseRepository = responseRepository;
        }

        public async Task<SessionDto?> GetChatHistoryAsync(Guid clientSessionId)
        {
            var session = await _sessionRepository.GetSessionByClientSessionIdAsync(clientSessionId);
            if (session == null) return null;

            return new SessionDto
            {
                Id = session.Id,
                ClientSessionId = session.ClientSessionId,
                CreatedAt = session.CreatedAt,
                LastActivityAt = session.LastActivityAt,
                Messages = session.Messages.Select(m => new MessageDto
                {
                    Id = m.Id,
                    Content = m.Content,
                    Sender = m.Sender,
                    CreatedAt = m.CreatedAt,
                    Rating = m.Rating?.ToString()
                }).ToList()
            };
        }

        public async Task<Session> GetOrCreateSessionAsync(Guid clientSessionId)
        {
            var session = await _sessionRepository.GetSessionByClientSessionIdAsync(clientSessionId);
            if (session != null) return session;

            session = new Session
            {
                ClientSessionId = clientSessionId,
                CreatedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow
            };

            await _sessionRepository.CreateSessionAsync(session);
            await _sessionRepository.SaveChangesAsync();

            return session;
        }

        public async Task<Message> SaveUserMessageAsync(Session session, string message)
        {
            var userMessage = new Message
            {
                Session = session,
                Sender = SenderType.User,
                Content = message,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddMessageAsync(userMessage);
            await _messageRepository.SaveChangesAsync();
            return userMessage;
        }

        public async Task<string> GetRandomResponseAsync()
        {
            var responses = await _responseRepository.GetAllResponsesAsync();
            if (responses == null || !responses.Any()) return "I'm currently unable to respond.";

            var random = new Random();
            return responses[random.Next(responses.Count)].Content;
        }

        public async Task<Message> SaveBotMessageAsync(Session session, string response)
        {
            var botMessage = new Message
            {
                Session = session,
                Sender = SenderType.Bot,
                Content = response,
                CreatedAt = DateTime.UtcNow
            };

            await _messageRepository.AddMessageAsync(botMessage);
            await _messageRepository.SaveChangesAsync();
            return botMessage;
        }

        public async Task RateMessageAsync(int messageId, MessageRating rating)
        {
            var message = await _messageRepository.GetMessageByIdAsync(messageId);
            if (message == null)
                throw new Exception($"Message with ID: {messageId} does not exist.");

            message.Rating = rating;
            await _messageRepository.SaveChangesAsync();
        }

        public async Task<Session> GetSessionByClientSessionIdAsync(Guid clientSessionId)
        {
            return await _sessionRepository.GetSessionByClientSessionIdAsync(clientSessionId);
        }

        public async Task<Session> CreateSessionAsync(Guid clientSessionId)
        {
            var session = new Session
            {
                ClientSessionId = clientSessionId,
                CreatedAt = DateTime.UtcNow,
                LastActivityAt = DateTime.UtcNow
            };

            await _sessionRepository.CreateSessionAsync(session);
            await _sessionRepository.SaveChangesAsync();

            return session;
        }

        public async Task SaveChangesAsync()
        {
            await _sessionRepository.SaveChangesAsync();
        }
    }
}
