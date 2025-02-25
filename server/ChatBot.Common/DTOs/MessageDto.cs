using ChatBot.Domain.Enums;

namespace ChatBot.Common.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public SenderType Sender { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Rating { get; set; }
        public string SessionId { get; set; }
    }
}
