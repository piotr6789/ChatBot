using ChatBot.Persistence.Entities;

namespace ChatBot.Persistence.Repositories
{
    public interface ISessionRepository
    {
        Task<Session> GetSessionByClientSessionIdAsync(Guid cookieId);
        Task CreateSessionAsync(Session session);
        Task SaveChangesAsync();
    }
}
