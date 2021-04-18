using System.Collections.Generic;

namespace Memento.Models.ViewModels.BrowseDecks
{
    public class BrowseDecksModel
    {
        public List<DeckModel> YourDecks { get; set; }

        public List<DeckModel> CreatedDecks { get; set; }

        public List<DeckModel> PopularDecks { get; set; }

        public string SearchValue { get; set; }
    }
}
