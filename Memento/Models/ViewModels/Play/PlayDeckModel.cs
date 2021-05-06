using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

namespace Memento.Models.ViewModels.Play
{
    public class PlayDeckModel
    {
        public string Name { get; set; }

        public long Id { get; set; }

        public ICollection<PlayCardModel> Cards { get; set; }

        public string CreatorName { get; set; }

        public string Difficulty { get; set; }
    }
}
