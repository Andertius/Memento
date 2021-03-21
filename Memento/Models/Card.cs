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

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        public string Word { get; set; }

        public string Description { get; set; }

        public string Transcription { get; set; }

        public byte[] Image { get; set; }

        public byte[] ShirtImage { get; set; }


        public User User { get; set; }

        public ICollection<Deck> Decks { get; set; }

        public ICollection<Tag> Tags { get; set; }
    }
}
