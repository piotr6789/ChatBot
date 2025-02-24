using ChatBot.Common.DTOs;
using ChatBot.Domain.Enums;

namespace ChatBot.Infrastructure.Services
{
    public interface IChatService
    {
        Task<SessionDto> GetChatHistoryAsync(string cookieId);
        Task<MessageDto> SendMessageAsync(string content, string cookieId);
        Task RateMessageAsync(int messageId, MessageRating rating);
    }
}
