namespace Memento.Models.ViewModels.DeckEditor
{
    public class CardModel
    {
        public long Id { get; set; }

        public DeckModel Deck { get; set; }

        public string Word { get; set; }

        public string Transcription { get; set; }

        public string Description { get; set; }

        public Difficulty Difficulty { get; set; }

        public byte[] Image { get; set; }
    }
}
