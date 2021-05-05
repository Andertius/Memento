using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Memento.Models.ViewModels
{
    public class StatisticsModel
    {
        public string Username { get; set; }
        public float HoursPerDay { get; set; }
        public float AverageHoursPerDay { get; set; }
        public int CardsPerDay { get; set; }
        public DateTime Date { get; set; }
        public int DayNumber { get; set; }
    }
}
