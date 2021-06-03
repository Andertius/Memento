using Memento.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Moq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace Memento.Tests
{
    public class GetStatsTest : IClassFixture<DbFixture>
    {
        private UserManager<User> _userManager;
        private ServiceProvider _serviceProvider;

        public GetStatsTest(DbFixture fixture)
        {
            var store = new Mock<IUserStore<User>>();
            _userManager = new UserManager<User>(store.Object, null, null, null, null, null, null, null, null);
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void Get30DaysTest()
        {
            var context = _serviceProvider.GetService<MementoDbContext>();

            var fakeUser = new User { UserName = "aasda", Email = "example@gmail.com" };

            var fakePass = "checkMe123";

            IdentityResult result = await _userManager.CreateAsync(fakeUser);

            var check = context.Users.Where(x => x.Email == fakeUser.Email).FirstOrDefault();

            Assert.NotNull(check);

        }
    }
}
