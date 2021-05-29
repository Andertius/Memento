using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Memento.Models;

using Xunit;

namespace Memento.Tests
{
    public class HomePageOnGet : IClassFixture<WebTestFixture>
    {
        public HomePageOnGet(WebTestFixture factory)
        {
            Client = factory.CreateClient();
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task ReturnsHomePageWithProductListing()
        {
            // Arrange & Act
            var response = await Client.GetAsync("/DeckEditor/ChooseDeck");
            response.EnsureSuccessStatusCode();
            var status = response.StatusCode;


            // Assert
            Assert.Equal(HttpStatusCode.Redirect, status);
        }
    }
}
