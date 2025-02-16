using System.ComponentModel.DataAnnotations;

namespace MauiHybridAuth.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Email Address")]
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public string LoginFailureMessage { get; set; } = "Invalid Email or Password. Please try again.";
        public string RegisterFailureMessage { get; set; } = "Invalid Email or Password. Please try again.";

    }
}
