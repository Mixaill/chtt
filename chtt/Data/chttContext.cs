using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace chtt.Models
{
    public class chttContext : DbContext
    {
        public DbSet<Conversation> Room { get; set; }
        public DbSet<Message> Message { get; set; }

        public chttContext (DbContextOptions<chttContext> options)
            : base(options)
        {
        }


    }
}
