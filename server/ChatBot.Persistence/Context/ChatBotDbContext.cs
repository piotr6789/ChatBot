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

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Response> Responses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SessionConfiguration());
            modelBuilder.ApplyConfiguration(new MessageConfiguration());
            modelBuilder.ApplyConfiguration(new ResponseConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public void EnsureDatabaseSeeded()
        {
            if (!this.Database.CanConnect()) return;

            if (!Responses.Any())
            {
                Responses.AddRange(
                    new Response { Content = "Lorem ipsum dolor sit amet." },
                    new Response { Content = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit." },
                    new Response { Content = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. 
                                              
                                              Phasellus ut risus nec libero ornare volutpat. 
                                              Nullam fermentum massa in orci placerat, non sodales libero ullamcorper." }
                );

                SaveChanges();
            }
        }
    }
}
