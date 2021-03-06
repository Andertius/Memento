using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

namespace Memento.Models
{
    public class User : IdentityUser
    {
        public byte[] ProfilePicture { get; set; }

        [Range(0, Int32.MaxValue)]
        public int CardsLearned { get; set; }

        [Range(0.0, Double.MaxValue)]
        public double TimeSpent { get; set; }

        [Range(0.0, Double.MaxValue)]
        public double AverageTime { get; set; }


        public ICollection<Deck> Decks { get; set; }

        public ICollection<Deck> CreatedDecks { get; set; }

        public ICollection<UserStats> Statistics { get; set; }

        public ICollection<UserRating> Ratings { get; set; }
    }
}
