namespace chtt.Models.MessagesViewModels
{
    public class UpdateViewModel
    {
        public int MessageId { get; set; }

        public string Content { get; set; }

        public UpdateViewModel(Message message)
        {
            this.MessageId = message.MessageId;
            this.Content = message.Content;
        }
    }
}
