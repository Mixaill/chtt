using System;
using System.Collections.Generic;
using System.Text;
using chtt.Controllers;
using chtt.Models.UsersViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace chtt.Tests
{
    public class UsersControllerTests
    {
        public UsersController GetController(bool setHttpContext = true)
        {
            var controller = new UsersController(TestInitializator.GetContext(), TestInitializator.GetUserManager().Object);
            if (setHttpContext)
            {
                controller.ControllerContext.HttpContext = TestInitializator.GetHttpContext().Object;
            }

            return controller;
        }

        [Fact]
        public void GetUsers_User()
        {
            var controller = GetController();
            var res = controller.GetUsers().Result as OkObjectResult;
            Assert.NotNull(res);
            Assert.Equal(200, res.StatusCode);

            var value = res.Value as List<GetViewModel>;
            Assert.NotNull(value);
            Assert.Equal(2,value.Count);
        }

        [Fact]
        public void GetUsers_NoUser()
        {
            var controller = GetController(false);
            var res = controller.GetUsers().Result as UnauthorizedResult;
            Assert.Equal(401, res.StatusCode);
        }

        [Fact]
        public void GetUser_Exists()
        {
            var controller = GetController();
            var res = controller.GetUser("test@test.it").Result as OkObjectResult;
            Assert.NotNull(res);
            Assert.Equal(200, res.StatusCode);

            var value = res.Value as GetViewModel;
            Assert.Equal("test@test.it", value.UserName);
        }

        [Fact]
        public void GetUser_NotExists()
        {
            var controller = GetController();
            var res = controller.GetUser("test32@test.it").Result as NotFoundResult;
            Assert.NotNull(res);
            Assert.Equal(404, res.StatusCode);
        }

        [Fact]
        public void PutUser_Valid()
        {
            var controller = GetController();
            var res = controller.PutUser("test@test.it").Result as NoContentResult;
            Assert.NotNull(res);
            Assert.Equal(204, res.StatusCode);
        }

        [Fact]
        public void PutUser_Forbidden()
        {
            var controller = GetController();
            var res = controller.PutUser("test2@test.it").Result as ForbidResult;
            Assert.NotNull(res);
        }
    }
}
