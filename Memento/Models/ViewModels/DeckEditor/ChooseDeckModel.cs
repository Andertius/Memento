using System.Collections.Generic;

namespace Memento.Models.ViewModels.DeckEditor
{
    public class ChooseDeckModel
    {
        public string UserName { get; set; }

        public string UserId { get; set; }

        public List<string> FilterTags { get; set; }

        public string FilterTagsString { get; set; }

        public string SearchFilter { get; set; }

        public string TagFilter { get; set; }

        public string TagToRemove { get; set; }

        public List<DeckModel> Decks { get; set; }
    }
}
