using System;
using System.Security.Claims;
using System.Threading.Tasks;

using Memento.Controllers;
using Memento.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using Xunit;

namespace Memento.Tests
{
    public class ControllerTest : IClassFixture<DbFixture>
    {
        private readonly UserManager<User> _userManager;
        private readonly ServiceProvider _serviceProvider;

        public ControllerTest(DbFixture fixture)
        {
            var store = new Mock<IUserPasswordStore<User>>();
            _userManager = new UserManager<User>(store.Object, null, null, null, null, null, null, null, null);
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async Task HomeController_Index_ReturnsIndexView()
        {
            //Arrange
            var context = _serviceProvider.GetService<MementoDbContext>();

            //Act
            var controller = new HomeController(_userManager, context)
            {
                ControllerContext = new ControllerContext()
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>()));
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
            ViewResult result = await controller.Index() as ViewResult;

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task HomeController_About_ReturnsAboutView()
        {
            //Arrange
            var context = _serviceProvider.GetService<MementoDbContext>();

            //Act
            var controller = new HomeController(_userManager, context)
            {
                ControllerContext = new ControllerContext()
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>()));
            controller.ControllerContext.HttpContext = new DefaultHttpContext() { User = user };
            ViewResult result = await controller.About() as ViewResult;

            //Assert
            Assert.NotNull(result);
        }
    }
}
