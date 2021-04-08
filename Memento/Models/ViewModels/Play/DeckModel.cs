﻿using Microsoft.AspNetCore.Http;

namespace Memento.Models.ViewModels.Play
{
    public class DeckModel
    {
        public string Name { get; set; }

        public long Id { get; set; }

        public IFormFile Thumb { get; set; }

        public bool ThumbRemoved { get; set; }

        public IFormFile Cover { get; set; }

        public bool CoverRemoved { get; set; }

        public Difficulty Difficulty { get; set; }

        public bool IsPublic { get; set; }
    }
}
