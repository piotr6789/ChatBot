using ChatBot.Persistence.Context;
using ChatBot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Persistence.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly ChatBotDbContext _context;

        public SessionRepository(ChatBotDbContext context)
        {
            _context = context;
        }

        public async Task<Session> GetSessionByClientSessionIdAsync(Guid clientSessionId)
        {
            return await _context.Sessions
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.ClientSessionId == clientSessionId);
        }

        public async Task CreateSessionAsync(Session session)
        {
            await _context.Sessions.AddAsync(session);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
