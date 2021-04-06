using System.Collections.Generic;

namespace Memento.Models.ViewModels.DeckEditor
{
    public class DeckEditorModel
    {
        public DeckModel Deck { get; set; }

        public List<CardModel> Cards { get; set; }
    }
}
