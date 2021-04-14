using Microsoft.AspNetCore.Http;

namespace Memento.Models.ViewModels.BrowseDecks
{
    public class DeckModel
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int CardNumber { get; set; }

        public IFormFile Thumb { get; set; }

        public double Rating { get; set; }

        public Difficulty Difficulty { get; set; }
    }
}
