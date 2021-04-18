using System.Collections.Generic;

namespace Memento.Models.ViewModels.BrowseDecks
{
    public class SearchedDecksModel
    {
        public List<DeckModel> Decks { get; set; }

        public string SearchValue { get; set; }
    }
}
