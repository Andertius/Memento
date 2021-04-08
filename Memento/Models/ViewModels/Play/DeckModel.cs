using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Memento.Models.ViewModels.Play
{
    public class DeckModel
    {
        public string Name { get; set; }

        public long Id { get; set; }

        public ICollection<CardModel> Cards { get; set; }
    }
}
