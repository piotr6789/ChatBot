using ChatBot.Persistence.Context;
using ChatBot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatBot.Persistence.Repositories
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly ChatBotDbContext _dbContext;

        public ResponseRepository(ChatBotDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Response> GetRandomResponseAsync()
        {
            var count = await _dbContext.Responses.CountAsync();
            if (count == 0)
                return null;

            var random = new Random();
            int randomIndex = random.Next(0, count);

            return await _dbContext.Responses
                .OrderBy(r => EF.Functions.Random())
                .Skip(randomIndex)
                .Take(1)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Response>> GetAllResponsesAsync()
        {
            return await _dbContext.Responses.ToListAsync();
        }
    }
}
