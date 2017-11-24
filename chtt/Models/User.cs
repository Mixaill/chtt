using Microsoft.AspNetCore.Identity;

namespace chtt.Models
{
    public class User : IdentityUser
    {
        public int UserId { get; set; }
    }
}
