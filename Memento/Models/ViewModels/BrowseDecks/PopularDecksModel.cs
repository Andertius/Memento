using System.Collections.Generic;

namespace Memento.Models.ViewModels.BrowseDecks
{
    public class PopularDecksModel
    {
        public List<DeckModel> PopularDecks { get; set; }

        public List<string> FilterTags { get; set; }

        public string SearchFilter { get; set; }

        public string TagFilter { get; set; }
    }
}
