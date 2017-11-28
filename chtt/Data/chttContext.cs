using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace chtt.Models
{
    public class chttContext : IdentityDbContext<User>
    {
        public DbSet<Conversation> Conversation { get; set; }

        public DbSet<ConversationUser> ConversationUser { get; set; }

        public DbSet<Message> Message { get; set; }

        public DbSet<User> User { get; set; }
     
        public chttContext (DbContextOptions<chttContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Conversation>()
                .HasOne(a => a.Author)
                .WithMany(b => b.Authorship);

            modelBuilder.Entity<ConversationUser>()
                .HasKey(cu => new { cu.ConversationId, cu.UserId });

            modelBuilder.Entity<ConversationUser>()
                .HasOne(cu => cu.Conversation)
                .WithMany("ConversationUsers");

            modelBuilder.Entity<ConversationUser>()
                .HasOne(cu => cu.User)
                .WithMany("ConversationUsers");
        }
    }
}
