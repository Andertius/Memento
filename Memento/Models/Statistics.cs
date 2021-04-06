using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Memento.Models
{
    public class Statistics
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [Range(0, 24.0)]
        public float HoursPerDay { get; set; }

        [Range(0, Int32.MaxValue)]
        public int CardsPerDay { get; set; }

        [Range(0, 24.0)]
        public float AverageHoursPerDay { get; set; }

        public DateTime Date { get; set; }

        public User User { get; set; }
    }
}
