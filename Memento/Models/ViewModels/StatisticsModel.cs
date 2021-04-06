using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Memento.Models.ViewModels
{
    public class StatisticsModel
    {
        public float HoursPerDay { get; set; }
        public float AverageHoursPerDay { get; set; }
        public long CardsPerDay { get; set; }
    }
}
