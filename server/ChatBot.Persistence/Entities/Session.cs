﻿namespace ChatBot.Persistence.Entities
{
    public class Session
    {
        public int Id { get; set; }
        public string CookieId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActivityAt { get; set; }

        public ICollection<Message> Messages { get; set; }
    }
}
