using System.Collections.Generic;

namespace Memento.Models.ViewModels.BrowseDecks
{
    public class CreatedDecksModel
    {
        public string Username { get; set; }

        public List<DeckModel> CreatedDecks { get; set; }

        public List<string> FilterTags { get; set; }

        public string SearchFilter { get; set; }

        public string TagFilter { get; set; }

        public string FilterTagsString { get; set; }

        public string TagToRemove { get; set; }
    }
}
