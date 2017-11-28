using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace chtt.Models.ConversationsViewModels
{
    public class UpdateViewModel
    {
        public int ConversationId { get; set; }

        [MinLength(1)]
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<string> Users { get; set; }

        public UpdateViewModel()
        {

        }

        public UpdateViewModel(Conversation conversation)
        {
            if (conversation == null)
                return;

            this.ConversationId = conversation.ConversationId;
            this.Name = conversation.Name;
            this.Description = conversation.Description;

            this.Users = new List<string>();
            foreach (var user in conversation.Users)
            {
                this.Users.Add(user.UserName);
            }
        }
    }
}
