using Microsoft.AspNetCore.Http;

namespace Memento.Models.ViewModels.DeckEditor
{
    public class CardModel
    {
        public string Username { get; set; }

        public long Id { get; set; }

        public long DeckId { get; set; }

        public IFormFile Image { get; set; }

        public string Word { get; set; }

        public string Description { get; set; }
    }
}
