using System;
using System.Collections.Generic;
using System.Text;
using chtt.Controllers;
using chtt.Models.MessagesViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace chtt.Tests
{
    public class MessagesControllerTests
    {
        public MessagesController GetController(bool setHttpContext = true)
        {
            var controller = new MessagesController(TestInitializator.GetContext(), TestInitializator.GetUserManager().Object);
            if (setHttpContext)
            {
                controller.ControllerContext.HttpContext = TestInitializator.GetHttpContext().Object;
            }

            return controller;
        }

        [Fact]
        public void GetMessage_Exists()
        {
            var controller = GetController();

            var res = controller.GetMessage(2).Result as OkObjectResult;
            Assert.NotNull(res);
            Assert.Equal(200, res.StatusCode);

            var value = res.Value as GetViewModel;
            Assert.NotNull(value);
            Assert.Equal("test@test.it", value.Author);
            Assert.Equal(TestInitializator.User.UserName, value.Author);
        }

        [Fact]
        public void GetMessage_Forbid()
        {
            var controller = GetController();

            var res = controller.GetMessage(3).Result as ForbidResult;
            Assert.NotNull(res);
        }

        [Fact]
        public void GetMessage_NotExists()
        {
            var controller = GetController();

            var res = controller.GetMessage(4).Result as NotFoundResult;
            Assert.NotNull(res);
            Assert.Equal(404, res.StatusCode);
        }

        [Fact]
        public void DeleteMessage_Exists()
        {
            var controller = GetController();

            var res = controller.DeleteMessage(2).Result as NoContentResult;
            Assert.NotNull(res);
            Assert.Equal(204, res.StatusCode);
        }

        [Fact]
        public void DeleteMessage_Forbid()
        {
            var controller = GetController();

            var res = controller.DeleteMessage(3).Result as ForbidResult;
            Assert.NotNull(res);
        }

        [Fact]
        public void DeleteMessage_NotExists()
        {
            var controller = GetController();

            var res = controller.GetMessage(4).Result as NotFoundResult;
            Assert.NotNull(res);
            Assert.Equal(404, res.StatusCode);
        }

        [Fact]
        public void PostMessage_Valid()
        {
            var controller = GetController();

            var c = new CreateMessageViewModel
            {
                ConversationId = 2,
                Content = "testtest"
            };
            var res = controller.PostMessage(c).Result as CreatedAtActionResult;
            Assert.NotNull(res);
        }

        [Fact]
        public void PostMessage_NotFound()
        {
            var controller = GetController();

            var c = new CreateMessageViewModel
            {
                ConversationId = 1,
                Content = "testtest"
            };
            var res = controller.PostMessage(c).Result as NotFoundResult;
            Assert.NotNull(res);
        }

        [Fact]
        public void PostMessage_Forbidden()
        {
            var controller = GetController();

            var c = new CreateMessageViewModel
            {
                ConversationId = 3,
                Content = "testtest"
            };
            var res = controller.PostMessage(c).Result as ForbidResult;
            Assert.NotNull(res);
        }
    }
}
