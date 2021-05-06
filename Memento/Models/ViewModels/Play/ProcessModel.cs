using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Memento.Models.ViewModels.Play
{
    public class ProcessModel
    {
        public long DeckId { get; set; }
        public string Username { get; set; }
        public PlayCardModel CurrentCard { get; set; }

        public bool ShowImages { get; set; }
    }
}
