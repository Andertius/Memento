using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Memento.Models
{
    public class Deck
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        [ForeignKey(nameof(Creator))]
        public long CreatorId { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsPublic { get; set; }

        [Column(TypeName = "decimal(2, 2)")]
        public double Rating { get; set; }

        [Range(0, Int32.MaxValue)]
        public int CardNumber { get; set; }

        public byte[] Cover { get; set; }

        public byte[] Thumbnail { get; set; }


        public User Creator { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<Tag> Tags { get; set; }

        public ICollection<Card> Cards { get; set; }
    }
}
