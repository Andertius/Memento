using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Memento.Models
{
    public class DeckTag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Name { get; set; }

        public string Description { get; set; }

        public int TotalCards { get; set; }

        public ICollection<Deck> Deck { get; set; }
    }
}
