namespace Memento.Models.ViewModels.Play
{
    public class PlayCardModel
    {
        public long Id { get; set; }

        public string Word { get; set; }

        public string Transcription { get; set; }

        public string Description { get; set; }

        public Difficulty Difficulty { get; set; }

        public byte[] Image { get; set; }
    }
}
