using System;
using System.ComponentModel.DataAnnotations;

namespace UserManagement.ViewModels;

    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
        
        [Compare("Password", ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
