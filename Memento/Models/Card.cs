using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Memento.Models
{
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Word { get; set; }

        public string Description { get; set; }

        public string Transcription { get; set; }

        public Difficulty Difficulty { get; set; }

        public byte[] Image { get; set; }

        public byte[] ShirtImage { get; set; }

        public ICollection<Deck> Decks { get; set; }

        public ICollection<CardTag> Tags { get; set; }
    }
}
