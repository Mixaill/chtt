using Microsoft.AspNetCore.Mvc;

namespace chtt.Models.MessagesViewModels
{
    public class CreateMessageViewModel
    {
        public int ConversationId { get; set; }

        public string Content { get; set; }

        public CreateMessageViewModel()
        {

        }

        public CreateMessageViewModel(Message message)
        {
            this.ConversationId = message.Conversation.ConversationId;
            this.Content = message.Content;
        }
    }
}
