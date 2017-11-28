using chtt.Service;

namespace chtt.Models
{

    public class ConversationUser : IJoinEntity<Conversation>, IJoinEntity<User>
    {
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; }
        Conversation IJoinEntity<Conversation>.Navigation
        {
            get => Conversation;
            set => Conversation = value;
        }

        public string UserId { get; set; }
        public User User { get; set; }
        User IJoinEntity<User>.Navigation
        {
            get => User;
            set => User = value;
        }

    }
}