using System.Threading.Tasks;

using Memento.Controllers;
using Memento.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace Memento.Tests
{
    [TestClass]
    public class DeckEditorControllerTests
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            var options = new DbContextOptionsBuilder<MementoDbContext>()
                .UseInMemoryDatabase(databaseName: "MementoDatabase")
                .Options;

            using var dbContext = new MementoDbContext(options);
            var userManager = new Mock<UserManager<User>>();

            var deckEditor = new DeckEditorController(userManager.Object, dbContext);

            var user = new User { UserName = "testUser", Email = "test-email@example.com" };
            await userManager.Object.CreateAsync(user, "password");

            dbContext.Decks.Add(new Deck
            {
                CreatorId = user.Id, 
                Name = "name",
            });

            var result = await deckEditor.ChooseCard(1);
        }
    }
}
