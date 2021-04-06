using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Memento.Models
{
    public class Settings
    {
        [Key]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [Range(0, 24.0)]
        public float HoursPerDay { get; set; }

        [Range(0, Int32.MaxValue)]
        public int CardsPerDay { get; set; }

        [Range(0, 2)]
        public int CardsOrder { get; set; }

        public bool DarkTheme { get; set; }

        public bool ShowImages { get; set; }

        public User User { get; set; }
    }
}
