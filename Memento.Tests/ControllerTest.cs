using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Memento.Controllers;
using Memento.Models;
using Memento.Models.ViewModels.BrowseDecks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Moq;

using Xunit;

namespace Memento.Tests
{
    public class ItemsControllerTest : IClassFixture<WebTestFixture>
    {

        public ItemsControllerTest()
        {
            ContextOptions = new DbContextOptionsBuilder<MementoDbContext>()
                   .UseInMemoryDatabase("InMemoryDbForTesting")
                   .Options;
        }

        protected DbContextOptions<MementoDbContext> ContextOptions { get; }

        public static Mock<UserManager<TUser>> MockUserManager<TUser>(List<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success).Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        [Fact]
        public async void Can_get_items()
        {
            using var context = new MementoDbContext(ContextOptions);
            var userManager = MockUserManager(new List<User>()).Object;
            var controller = new BrowseDecksController(userManager, context);

            var user = new User { UserName = "aasda", Email = "example@gmail.com" };
            IdentityResult result = await userManager.CreateAsync(user, "Secret123");

            Assert.Equal(user.UserName, context.Users.Find(user).UserName);
        }

        [Fact]
        public async void Check()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(ClaimTypes.NameIdentifier, "SomeValueHere"),
                                        new Claim(ClaimTypes.Name, "gunnar@somecompany.com")
                                        // other required and custom claims
                                   }, "TestAuthentication"));
            

            using var context = new MementoDbContext(ContextOptions);
            var userManager = MockUserManager(new List<User>()).Object;

            var controller = new HomeController(userManager, context);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

            var check = new HomeController(userManager, context);

            var result = await controller.Index() as ViewResult;

            Assert.True(result.ViewName is null || result.ViewName == "Index");

        }
    }
}
