using ChatBot.Persistence.Entities;

namespace ChatBot.Persistence.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> GetMessageByIdAsync(int messageId);
        Task AddMessageAsync(Message message);
        Task SaveChangesAsync();
    }
}
