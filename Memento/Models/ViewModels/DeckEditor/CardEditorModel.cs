namespace Memento.Models.ViewModels.DeckEditor
{
    public class CardEditorModel
    {
        public DeckModel Deck { get; set; }

        public CardModel Card { get; set; }

        public string UserName { get; set; }
    }
}
