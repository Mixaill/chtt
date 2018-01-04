using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;

using Xunit;

using chtt.Controllers;
using chtt.Models.ConversationsViewModels;

namespace chtt.Tests
{
    public class ConversationsControllerTests
    {
        public ConversationsController GetController(bool setHttpContext = true)
        {
            var controller = new ConversationsController(TestInitializator.GetContext(), TestInitializator.GetUserManager().Object);
            if (setHttpContext)
            {
                controller.ControllerContext.HttpContext = TestInitializator.GetHttpContext().Object;
            }

            return controller;
        }


        [Fact]
        public void GetConversations_User()
        {
            var controller = GetController();
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
            var controller = GetController(false);
            var res = controller.GetConversations().Result as UnauthorizedResult;
            Assert.Equal(401, res.StatusCode);
        }

        [Fact]
        public void GetConversation_Exists()
        {
            var controller = GetController();

            var res = controller.GetConversation(2).Result as OkObjectResult;
            Assert.NotNull(res);
            Assert.Equal(200, res.StatusCode);

            var value = res.Value as GetViewModel;
            Assert.NotNull(value);
            Assert.Equal("test name", value.Name);
            Assert.Equal(TestInitializator.User.UserName,value.Author);
        }

        [Fact]
        public void GetConversation_NonExists()
        {
            var controller = GetController();

            var res = controller.GetConversation(100500).Result as NotFoundResult;
            Assert.NotNull(res);
            Assert.Equal(404, res.StatusCode);
        }

        [Fact]
        public void GetConversation_Forbidden()
        {
            var controller = GetController();

            var res = controller.GetConversation(3).Result as ForbidResult;
            Assert.NotNull(res);
        }

        [Fact]
        public void PostConversation()
        {
            var controller = GetController();

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
            var controller = GetController();

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
            var controller = GetController();

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

        [Fact]
        public void DeleteConversation_Exists()
        {
            var controller = GetController();
            var res = controller.DeleteConversation(2).Result as NoContentResult;
            Assert.NotNull(res);
            Assert.Equal(204, res.StatusCode);
        }

        [Fact]
        public void DeleteConversation_Forbidden()
        {
            var controller = GetController();
            var res = controller.DeleteConversation(3).Result as ForbidResult;
            Assert.NotNull(res);
        }

        [Fact]
        public void DeleteConversation_NotExists()
        {
            var controller = GetController();
            var res = controller.DeleteConversation(4).Result as NotFoundResult;
            Assert.NotNull(res);
            Assert.Equal(404, res.StatusCode);
        }

    }
}
