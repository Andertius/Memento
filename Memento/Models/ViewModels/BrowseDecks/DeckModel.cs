using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

namespace Memento.Models.ViewModels.BrowseDecks
{
    public class DeckModel
    {
        public string Username { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }

        public string CreatorName { get; set; }

        public int CardNumber { get; set; }

        public IFormFile Thumb { get; set; }

        public double AverageRating { get; set; }

        public double UserRating { get; set; }

        public int RatingNumber { get; set; }

        public Difficulty Difficulty { get; set; }

        public bool HasInCollection { get; set; }

        public List<string> Tags { get; set; }
    }
}
