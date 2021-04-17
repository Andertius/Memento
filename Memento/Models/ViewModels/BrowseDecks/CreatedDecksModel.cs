using System.Collections.Generic;

namespace Memento.Models.ViewModels.BrowseDecks
{
    public class CreatedDecksModel
    {
        public List<DeckModel> CreatedDecks { get; set; }

        public string SearchFilter { get; set; }
    }
}
