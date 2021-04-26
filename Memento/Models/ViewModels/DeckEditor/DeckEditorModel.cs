using System.Collections.Generic;

namespace Memento.Models.ViewModels.DeckEditor
{
    public class DeckEditorModel
    {
        public DeckModel Deck { get; set; }

        public string CardSearchFilter { get; set; }

        public List<TagModel> Tags { get; set; }

        public string TagInput { get; set; }

        public List<CardEditorModel> Cards { get; set; }
    }
}
