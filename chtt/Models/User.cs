using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

using chtt.Service;

namespace chtt.Models
{
    public class User : IdentityUser
    {
        public  User() => Conversations = new JoinCollectionFacade<Conversation, User, ConversationUser>(this, ConversationUsers);

        public ICollection<Conversation> Authorship { get; set; }

        private ICollection<ConversationUser> ConversationUsers { get; } = new List<ConversationUser>();

        [NotMapped]
        public ICollection<Conversation> Conversations { get; }

        public DateTime LastOnline { get; set; }
    }
}
