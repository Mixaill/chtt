using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using chtt.Service;

namespace chtt.Models
{
    public class Conversation
    {
        public Conversation() => Users =  new JoinCollectionFacade<User, Conversation, ConversationUser>(this, ConversationUsers);

        public int ConversationId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public User Author { get; set; }

        private ICollection<ConversationUser> ConversationUsers { get; } = new List<ConversationUser>();

        [NotMapped]
        public ICollection<User> Users { get; }
    }
}
 