using System.ComponentModel.DataAnnotations;

namespace Memento.Models.ViewModels
{
    public class ProfileSettingsModel
    {
        public string Email { get; set; }

        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords should match.")]
        public string PasswordConfirm { get; set; }
    }
}
