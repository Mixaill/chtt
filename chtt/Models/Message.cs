using System;

namespace chtt.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        public DateTime Timestamp { get; set; }

        public Conversation Conversation { get; set; }

        public User Author { get; set; }

        public string Content { get; set; }
    }
}
