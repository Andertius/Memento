using System.Collections.Generic;

namespace Memento.Models.ViewModels.BrowseDecks
{
    public class YourCollectionModel
    {
        public List<DeckModel> YourDecks { get; set; }

        public List<string> FilterTags { get; set; }

        public string SearchFilter { get; set; }

        public string TagFilter { get; set; }
    }
}
