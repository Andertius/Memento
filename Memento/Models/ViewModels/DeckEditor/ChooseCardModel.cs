using System.Collections.Generic;

namespace Memento.Models.ViewModels.DeckEditor
{
    public class ChooseCardModel
    {
        public string Username { get; set; }

        public long CardId { get; set; }

        public long DeckId { get; set; }

        public List<CardModel> Cards { get; set; }

        public List<string> FilterTags { get; set; }

        public string FilterTagsString { get; set; }

        public string SearchFilter { get; set; }

        public string TagFilter { get; set; }

        public string TagToRemove { get; set; }
    }
}
