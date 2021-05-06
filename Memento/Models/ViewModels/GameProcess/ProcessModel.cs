using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Memento.Models.ViewModels.GameProcess
{
    public class ProcessModel
    {
        public long DeckId { get; set; }
        public string Username { get; set; }
        public Play.PlayCardModel CurrentCard { get; set; }

        public bool ShowImages { get; set; }
    }
}
