using System.ComponentModel.DataAnnotations;

namespace Memento.Models.ViewModels
{
    public class CreateDeckModel
    {
        [Required(ErrorMessage = "Deck name cannot be empty.")]
        public string Name { get; set; }
    }
}
