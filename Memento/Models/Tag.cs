using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Memento.Models
{
    public class Tag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int TotalCards { get; set; }

        public ICollection<Deck> Decks { get; set; }

        public ICollection<Card> Cards { get; set; }
    }
}
