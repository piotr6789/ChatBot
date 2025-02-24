using ChatBot.Persistence.Entities;

namespace ChatBot.Persistence.Repositories
{
    public interface ISessionRepository
    {
        Task<Session> GetSessionByCookieIdAsync(string cookieId);
        Task CreateSessionAsync(Session session);
        Task SaveChangesAsync();
    }
}
