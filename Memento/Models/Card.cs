using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Memento.Models
{
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public ICollection<Deck> DeckID { get; set; }

        [ForeignKey(nameof(User))]
        public long UserId { get; set; }

        public ICollection<Tag> Tags { get; set; }

        public string Word { get; set; }
        public string Description { get; set; }
        public string Transcription { get; set; }

        public ICollection<byte> Image { get; set; }
        public ICollection<byte> ShirtImage { get; set; }
    }
}
