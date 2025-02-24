namespace ChatBot.Common.DTOs
{
    public class SessionDto
    {
        public int Id { get; set; }
        public string CookieId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivityAt { get; set; }
        public IEnumerable<MessageDto> Messages { get; set; }
    }
}
