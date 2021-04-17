using System.Collections.Generic;

namespace Memento.Models
{
    public class DeckDto
    {
        public long Id { get; set; }

        public string CreatorId { get; set; }

        public string Name { get; set; }

        public Difficulty Difficulty { get; set; }

        public bool IsPublic { get; set; }

        public int CardNumber { get; set; }

        public ICollection<Card> Cards { get; set; }

        public ICollection<DeckTag> Tags { get; set; }

        public ICollection<UserRating> Ratings { get; set; }
    }
}
