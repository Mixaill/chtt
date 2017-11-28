using System.Collections.Generic;

namespace chtt.Models.ConversationsViewModels
{
    public class GetViewModel
    {
        public int ConversationId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public ICollection<string> Users { get; set; }

        public GetViewModel(Conversation conversation)
        {
            this.ConversationId = conversation.ConversationId;
            this.Name = conversation.Name;
            this.Description = conversation.Description;
            this.Author = conversation.Author.UserName;

            this.Users = new List<string>();
            foreach (var user in conversation.Users)
            {
                this.Users.Add(user.UserName);
            }
        }
    }
}
