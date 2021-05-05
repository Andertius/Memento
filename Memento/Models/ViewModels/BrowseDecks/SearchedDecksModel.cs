using System.Collections.Generic;

namespace Memento.Models.ViewModels.BrowseDecks
{
    public class SearchedDecksModel
    {
        public string Username { get; set; }

        public List<DeckModel> Decks { get; set; }

        public List<string> FilterTags { get; set; }

        public string FilterTagsString { get; set; }

        public string SearchValue { get; set; }

        public string TagFilter { get; set; }

        public string TagToRemove { get; set; }
    }
}
