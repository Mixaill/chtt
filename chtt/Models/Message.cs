using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace chtt.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        public DateTime Timestamp { get; set; }

        public Conversation Conversation { get; set; }
        public int ConversationId { get; set; }

        public User User { get; set; }
        public int UserId { get; set; }

        public string Text { get; set; }

        public Message()
        {

        }
    }
}
