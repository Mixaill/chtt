using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Moq;

using chtt.Models;

namespace chtt.Tests
{
    class TestInitializator
    {
        public static User User;

        private static Random random = new Random();
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static chttContext GetContext()
        {
            var builder = new DbContextOptionsBuilder<chttContext>()
                .UseInMemoryDatabase(RandomString(10))
                .EnableSensitiveDataLogging();

            var context = new chttContext(builder.Options);

            User = new User
            {
                UserName = "test@test.it",
                NormalizedUserName = "TEST@TEST.IT",
                Id = Guid.NewGuid().ToString(),
                Email = "test@test.it"
            };
            context.User.Add(User);

            var _otherUser = new User
            {
                UserName = "test2@test.it",
                Id = Guid.NewGuid().ToString(),
                Email = "test2@test.it"
            };
            context.User.Add(_otherUser);

            var _conversation = new Conversation
            {
                ConversationId = 2,
                Author = User,
                Description = "test description",
                Name = "test name"
            };
            context.Conversation.Add(_conversation);

            var _otherConversation = new Conversation
            {
                ConversationId = 3,
                Author = _otherUser,
                Description = "test description2",
                Name = "test name2"
            };
            context.Conversation.Add(_otherConversation);

            context.ConversationUser.Add(new ConversationUser
            {
                Conversation = _conversation,
                User = User
            });

            var _message = new Message()
            {
                MessageId = 2,
                Author = User,
                Content = "test message",
                Conversation = _conversation,
                Timestamp = DateTime.UtcNow
            };
            context.Message.Add(_message);

            var _otherMessage = new Message()
            {
                MessageId = 3,
                Author = _otherUser,
                Content = "test message2",
                Conversation = _otherConversation,
                Timestamp = DateTime.UtcNow
            };
            context.Message.Add(_otherMessage);

            context.SaveChanges();

            return context;
        }

        public static Mock<UserManager<User>> GetUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            var _userManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _userManager
                .Setup(mgr => mgr.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(User));
            return _userManager;
        }

        public static Mock<HttpContext> GetHttpContext()
        {
            var _httpContext = new Mock<HttpContext>();
            _httpContext
                .Setup(x => x.User)
                .Returns(new ClaimsPrincipal());
            return _httpContext;
        }

    }
}
