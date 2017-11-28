using System.ComponentModel.DataAnnotations;

namespace chtt.Models.ConversationsViewModels
{
    public class CreateViewModel
    {
        [MinLength(1)]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
