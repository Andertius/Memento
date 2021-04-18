using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Memento.Models.ViewModels
{
    public class ProfileSettingsModel
    {
        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords should match.")]
        public string PasswordConfirm { get; set; }

        public IFormFile ProfilePicture { get; set; }

        public bool NoPicture { get; set; }
    }
}
