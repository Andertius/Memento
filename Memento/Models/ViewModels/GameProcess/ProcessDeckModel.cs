using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Memento.Models.ViewModels.GameProcess
{
    public class ProcessDeckModel
    {
        public long Id { get; set; }
        public ICollection<Play.PlayCardModel> Cards { get; set; }
    }
}
