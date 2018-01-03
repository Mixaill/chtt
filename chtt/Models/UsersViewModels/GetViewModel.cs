using System;

namespace chtt.Models.UsersViewModels
{
    public class GetViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime LastOnline { get; set; }

        public GetViewModel(User user)
        {
            this.Id = user.Id;
            this.UserName = user.UserName;
            this.Email = user.Email;
            this.LastOnline = user.LastOnline;
        }
    }
}
