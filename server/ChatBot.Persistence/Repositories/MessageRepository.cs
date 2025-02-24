using ChatBot.Persistence.Context;
using ChatBot.Persistence.Entities;

namespace ChatBot.Persistence.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ChatBotDbContext _context;

        public MessageRepository(ChatBotDbContext context)
        {
            _context = context;
        }

        public async Task<Message> GetMessageByIdAsync(int messageId)
        {
            return await _context.Messages.FindAsync(messageId);
        }

        public async Task AddMessageAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
