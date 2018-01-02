using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

using chtt.Controllers;
using chtt.Models;
using chtt.Models.ConversationsViewModels;

namespace chtt.Tests
{
    public class ConversationsControllerTests
    {
        public ConversationsControllerTests()
        {
            InitContext();
            InitUserManager();
            InitHttpContext();
        }

        private chttContext _context;
        private User _user;
        private Mock<UserManager<User>> _userManager;
        private Mock<HttpContext> _httpContext;
        private ConversationsController _controller;

        #region Initialization
        private void InitContext()
        {
            var builder = new DbContextOptionsBuilder<chttContext>()
                .UseInMemoryDatabase()
                .EnableSensitiveDataLogging();

            var context = new chttContext(builder.Options);

            _user = new User
            {
                UserName = "Test",
                Id = Guid.NewGuid().ToString(),
                Email = "test@test.it"
            };
            context.User.Add(_user);

            var _conversation = new Conversation
            {
                ConversationId = 1,
                Author = _user,
                Description = "test description",
                Name = "test name"
            };
            context.Conversation.Add(_conversation);

            context.ConversationUser.Add(new ConversationUser
            {
                Conversation = _conversation,
                User = _user
            });

            context.SaveChanges();

            _context = context;
        }

        private void InitUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _userManager
                .Setup(mgr => mgr.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_user));

        }

        private void InitHttpContext()
        {
            _httpContext = new Mock<HttpContext>();
            _httpContext
                .Setup(x => x.User)
                .Returns(new ClaimsPrincipal());
        }
        #endregion


        [Fact]
        public void GetConversations_User()
        {
            var controller = new ConversationsController(_context, _userManager.Object)
                {
                    ControllerContext = { HttpContext = _httpContext.Object }
                };

            var res = controller.GetConversations().Result as OkObjectResult;
            Assert.NotNull(res);
            Assert.Equal(200, res.StatusCode);

            var value = res.Value as List<GetViewModel>;
            Assert.NotNull(value);
            Assert.Single(value);
        }

        [Fact]
        public void GetConversations_NoUser()
        {
            var controller = new ConversationsController(_context, _userManager.Object);
            var res = controller.GetConversations().Result as UnauthorizedResult;
            Assert.Equal(401, res.StatusCode);
        }

    }
}
