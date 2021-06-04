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
            var store = new Mock<IUserPasswordStore<User>>();
            _userManager = new UserManager<User>(store.Object, null, null, null, null, null, null, null, null);

            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public void Get30DaysTest()
        {
            var context = _serviceProvider.GetService<MementoDbContext>();

            var fakeUser = new User { UserName = "aasda", Email = "example@gmail.com" };

            var fakePass = "checkMe123";

            context.Users.Add(fakeUser);
            context.SaveChanges();

            var check = context.Users.Where(x => x.Email == fakeUser.Email).FirstOrDefault();

            DateTime comparator = DateTime.UtcNow.AddDays(-31);

            var newStats = new UserStats
            {
                UserId = check.Id,
                HoursPerDay = 0,
                CardsPerDay = 0,
                AverageHoursPerDay = 0,
                Date = comparator,
            };

            context.Statistics.Add(newStats);
            context.SaveChanges();
            //IdentityResult result = await _userManager.CreateAsync(fakeUser, fakePass);

            comparator = DateTime.UtcNow.AddDays(-30);

            var newStats2 = new UserStats
            {
                UserId = check.Id,
                HoursPerDay = 0,
                CardsPerDay = 0,
                AverageHoursPerDay = 0,
                Date = comparator,
            };

            context.Statistics.Add(newStats2);
            context.SaveChanges();

            comparator = DateTime.UtcNow.AddDays(-15);

            var newStats3 = new UserStats
            {
                UserId = check.Id,
                HoursPerDay = 0,
                CardsPerDay = 0,
                AverageHoursPerDay = 0,
                Date = comparator,
            };

            context.Statistics.Add(newStats3);
            context.SaveChanges();


            var stats = new GetUserStats(context);
            var lst = stats.GetCards(check.Id, 30);

            context.Statistics.Remove(newStats);
            context.Statistics.Remove(newStats2);
            context.Statistics.Remove(newStats3);
            context.SaveChanges();

            context.Users.Remove(fakeUser);
            context.SaveChanges();

            Assert.Equal(3, lst.Count);

        }

        [Fact]
        public void Get7DaysTest()
        {
            var context = _serviceProvider.GetService<MementoDbContext>();

            var fakeUser = new User { UserName = "aasda", Email = "example@gmail.com" };

            var fakePass = "checkMe123";

            context.Users.Add(fakeUser);
            context.SaveChanges();

            var check = context.Users.Where(x => x.Email == fakeUser.Email).FirstOrDefault();

            DateTime comparator = DateTime.UtcNow.AddDays(-31);

            var newStats = new UserStats
            {
                UserId = check.Id,
                HoursPerDay = 0,
                CardsPerDay = 0,
                AverageHoursPerDay = 0,
                Date = comparator,
            };

            context.Statistics.Add(newStats);
            context.SaveChanges();
            //IdentityResult result = await _userManager.CreateAsync(fakeUser, fakePass);

            comparator = DateTime.UtcNow.AddDays(-8);

            var newStats2 = new UserStats
            {
                UserId = check.Id,
                HoursPerDay = 0,
                CardsPerDay = 0,
                AverageHoursPerDay = 0,
                Date = comparator,
            };

            context.Statistics.Add(newStats2);
            context.SaveChanges();

            comparator = DateTime.UtcNow.AddDays(-5);

            var newStats3 = new UserStats
            {
                UserId = check.Id,
                HoursPerDay = 0,
                CardsPerDay = 0,
                AverageHoursPerDay = 0,
                Date = comparator,
            };

            context.Statistics.Add(newStats3);
            context.SaveChanges();


            var stats = new GetUserStats(context);
            var lst = stats.GetCards(check.Id, 7);

            context.Statistics.Remove(newStats);
            context.Statistics.Remove(newStats2);
            context.Statistics.Remove(newStats3);
            context.SaveChanges();

            context.Users.Remove(fakeUser);
            context.SaveChanges();

            Assert.Equal(2, lst.Count);

        }
    }
}
