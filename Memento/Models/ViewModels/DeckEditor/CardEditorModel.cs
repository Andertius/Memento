using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

namespace Memento.Models.ViewModels.DeckEditor
{
    public class CardEditorModel
    {
        public long Id { get; set; }

        public long ExistingId { get; set; }

        public long DeckId { get; set; }

        [Required(ErrorMessage = "A card cannot be without the word!")]
        public string Word { get; set; }

        public string Transcription { get; set; }

        [Required(ErrorMessage = "A card needs a description for the word!")]
        public string Description { get; set; }

        public string SearchFilter { get; set; }

        public Difficulty Difficulty { get; set; }

        public IFormFile Image { get; set; }

        public bool ImageRemoved { get; set; }

        public string TagInput { get; set; }

        public ICollection<TagModel> Tags { get; set; }

        public ICollection<string> Errors { get; set; }
    }
}
