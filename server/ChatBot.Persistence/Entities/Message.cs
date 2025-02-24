using ChatBot.Domain.Enums;

namespace ChatBot.Persistence.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }
        public SenderType Sender { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public MessageRating? Rating { get; set; }
    }
}
