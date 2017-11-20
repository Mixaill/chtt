using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace chtt.Models
{
    public class Conversation
    {
        public int ConversationId { get; set; }

        public string Name { get; set; }

        public List<User> Users { get; set; }

        public Conversation()
        {
        }
    }
}
