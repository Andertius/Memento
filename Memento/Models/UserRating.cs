using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Memento.Models
{
    public class UserRating
    {
        [Key, Column(Order = 0)]
        public long DeckId { get; set; }

        [Key, Column(Order = 1)]
        public string UserId { get; set; }

        public int Rating { get; set; }


        public Deck Deck { get; set; }

        public User User { get; set; }
    }
}
