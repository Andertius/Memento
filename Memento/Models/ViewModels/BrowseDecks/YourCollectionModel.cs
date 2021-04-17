using System.Collections.Generic;

namespace Memento.Models.ViewModels.BrowseDecks
{
    public class YourCollectionModel
    {
        public List<DeckModel> YourDecks { get; set; }

        public string SearchFilter { get; set; }
    }
}
