using System;
using System.Collections.Generic;
using System.Linq;
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

        private static Random random = new Random();
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private User _user;

        #region Initialization
        private chttContext InitContext()
        {
            var builder = new DbContextOptionsBuilder<chttContext>()
                .UseInMemoryDatabase(RandomString(10))
                .EnableSensitiveDataLogging();

            var context = new chttContext(builder.Options);

            _user = new User
            {
                UserName = "Test",
                Id = Guid.NewGuid().ToString(),
                Email = "test@test.it"
            };
            context.User.Add(_user);

            var _otherUser = new User
            {
                UserName = "Test2",
                Id = Guid.NewGuid().ToString(),
                Email = "test2@test.it"
            };
            context.User.Add(_otherUser);

            var _conversation = new Conversation
            {
                ConversationId = 2,
                Author = _user,
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
                User = _user
            });

            context.SaveChanges();
          
            return context;
        }

        private Mock<UserManager<User>> InitUserManager()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            var _userManager = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _userManager
                .Setup(mgr => mgr.FindByNameAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_user));
            return _userManager;
        }

        private Mock<HttpContext> InitHttpContext()
        {
            var _httpContext = new Mock<HttpContext>();
            _httpContext
                .Setup(x => x.User)
                .Returns(new ClaimsPrincipal());
            return _httpContext;
        }

        private ConversationsController GetConversationController(bool setHttpContext = true)
        {
            var controller = new ConversationsController(InitContext(), InitUserManager().Object);
            if (setHttpContext)
            {
                controller.ControllerContext.HttpContext = InitHttpContext().Object;
            }

            return controller;
        }
        
        #endregion


        [Fact]
        public void GetConversations_User()
        {
            var controller = GetConversationController();
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
            var controller = GetConversationController(false);
            var res = controller.GetConversations().Result as UnauthorizedResult;
            Assert.Equal(401, res.StatusCode);
        }

        [Fact]
        public void GetConversation_Exists()
        {
            var controller = GetConversationController();

            var res = controller.GetConversation(2).Result as OkObjectResult;
            Assert.NotNull(res);
            Assert.Equal(200, res.StatusCode);

            var value = res.Value as GetViewModel;
            Assert.NotNull(value);
            Assert.Equal("test name", value.Name);
            Assert.Equal(_user.UserName,value.Author);
        }

        [Fact]
        public void GetConversation_NonExists()
        {
            var controller = GetConversationController();

            var res = controller.GetConversation(100500).Result as NotFoundResult;
            Assert.NotNull(res);
            Assert.Equal(404, res.StatusCode);
        }

        [Fact]
        public void GetConversation_Forbidden()
        {
            var controller = GetConversationController();

            var res = controller.GetConversation(3).Result as ForbidResult;
            Assert.NotNull(res);
        }

        [Fact]
        public void PostConversation()
        {
            var controller = GetConversationController();

            var c = new CreateViewModel
            {
                Name = "testname_post",
                Description = "testdescr_post"
            };
            var res = controller.PostConversation(c).Result;
            Assert.NotNull(res);
        }

        [Fact]
        public void PutConversation_Valid()
        {
            var controller = GetConversationController();

            var u = new UpdateViewModel
            {
                ConversationId = 2,
                Description = "test description",
                Name = "test name",
                Users = new List<string>() { "Test" }
            };
            var res = controller.PutConversation(2, u).Result as NoContentResult;
            Assert.NotNull(res);
            Assert.Equal(204,res.StatusCode);
        }

        [Fact]
        public void PutConversation_InvalidID()
        {
            var controller = GetConversationController();

            var u = new UpdateViewModel
            {
                ConversationId = 22,
                Description = "test description",
                Name = "test name",
                Users = new List<string>() { "Test" }
            };
            var res = controller.PutConversation(2, u).Result as BadRequestResult;
            Assert.NotNull(res);
            Assert.Equal(400, res.StatusCode);
        }


    }
}
