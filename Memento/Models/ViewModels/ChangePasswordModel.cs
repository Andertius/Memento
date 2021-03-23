using System.ComponentModel.DataAnnotations;

namespace Memento.Models.ViewModels
{
    public class ChangePasswordModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }
}
