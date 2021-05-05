﻿using System.Collections.Generic;

namespace Memento.Models.ViewModels.Play
{
    public class PlayDeckModel
    {
        public string Name { get; set; }

        public long Id { get; set; }

        public ICollection<PlayCardModel> Cards { get; set; }
    }
}
