using ChatBot.Persistence.Entities;

namespace ChatBot.Persistence.Repositories
{
    public interface IResponseRepository
    {
        Task<Response> GetRandomResponseAsync();
        Task<List<Response>> GetAllResponsesAsync();
    }
}
