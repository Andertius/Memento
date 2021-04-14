﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Memento.Models.ViewModels.Play
{
    public class PlayModel
    {
        public DeckModel PickedDeck { get; set; }

        public CardModel CurrentCard { get; set; }
    }
}