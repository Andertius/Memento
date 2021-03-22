using System.ComponentModel.DataAnnotations;

namespace Memento.Models.ViewModel
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
