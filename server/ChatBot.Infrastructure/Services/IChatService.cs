using ChatBot.Common.DTOs;
using ChatBot.Domain.Enums;
using ChatBot.Persistence.Entities;

namespace ChatBot.Infrastructure.Services
{
    public interface IChatService
    {
        Task<SessionDto?> GetChatHistoryAsync(Guid clientSessionId);
        Task<Session> GetOrCreateSessionAsync(Guid clientSessionId);
        Task<Message> SaveUserMessageAsync(Session session, string message);
        Task<Message> SaveBotMessageAsync(Session session, string response);
        Task<string> GetRandomResponseAsync();
        Task RateMessageAsync(int messageId, MessageRating rating);
        Task<Session> GetSessionByClientSessionIdAsync(Guid clientSessionId);
        Task<Session> CreateSessionAsync(Guid clientSessionId);
        Task SaveChangesAsync();
    }
}
