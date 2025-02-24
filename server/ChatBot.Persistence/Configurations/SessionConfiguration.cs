using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ChatBot.Persistence.Entities;

namespace ChatBot.Persistence.Configurations
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.ToTable("Sessions");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CookieId)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(c => c.LastActivityAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()");

            builder.HasMany(c => c.Messages)
                   .WithOne(m => m.Session)
                   .HasForeignKey(m => m.SessionId);
        }
    }
}
