using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Memento.Models.ViewModels.Play
{
    public class ProcessDeckModel
    {
        public long Id { get; set; }
        public ICollection<PlayCardModel> Cards { get; set; }
    }
}
