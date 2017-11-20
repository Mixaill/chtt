using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using chtt.Models;

namespace chtt.Models
{
    public class chttContext : IdentityDbContext<User>
    {
        public DbSet<Conversation> Room { get; set; }
        public DbSet<Message> Message { get; set; }

        public chttContext (DbContextOptions<chttContext> options)
            : base(options)
        {
        }

        public DbSet<chtt.Models.User> User { get; set; }


    }
}
