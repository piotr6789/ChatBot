using Microsoft.EntityFrameworkCore;
using ChatBot.Persistence.Entities;
using ChatBot.Persistence.Configurations;

namespace ChatBot.Persistence.Context
{
    public class ChatBotDbContext : DbContext
    {
        public ChatBotDbContext(DbContextOptions<ChatBotDbContext> options)
            : base(options)
        { }

        public DbSet<Session> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ConversationConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
