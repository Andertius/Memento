using System.ComponentModel.DataAnnotations;

namespace Memento.Models.ViewModels
{
    public class CreateDeckModel
    {
        public string Username { get; set; }

        [Required(ErrorMessage = "Deck name cannot be empty.")]
        public string Name { get; set; }
    }
}
