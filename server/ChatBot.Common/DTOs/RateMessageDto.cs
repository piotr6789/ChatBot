using ChatBot.Domain.Enums;

namespace ChatBot.Common.DTOs
{
    public class RateMessageDto
    {
        public int MessageId { get; set; }
        public MessageRating Rating { get; set; }
    }
}
