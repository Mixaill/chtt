using System;

namespace chtt.Models.MessagesViewModels
{
    public class GetViewModel
    {
        public int MessageId {get; set;}

        public DateTime Timestamp { get; set; }

        public int ConversationId { get; set; }

        public string Author { get; set; }

        public string Content { get; set; }

        public GetViewModel(Message message)
        {
            this.MessageId = message.MessageId;
            this.Timestamp = message.Timestamp;
            this.ConversationId = message.Conversation.ConversationId;
            this.Author = message.Author.UserName;
            this.Content = message.Content;
        }
    }

}
