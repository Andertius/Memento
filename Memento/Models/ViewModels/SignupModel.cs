﻿using System.ComponentModel.DataAnnotations;

namespace Memento.Models.ViewModels
{
    public class SignupModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}