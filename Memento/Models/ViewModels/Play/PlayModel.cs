using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Memento.Models.ViewModels.Play
{
    public class PlayModel
    {
        public PlayDeckModel PickedDeck { get; set; }

        public PlayCardModel CurrentCard { get; set; }
    }
}
