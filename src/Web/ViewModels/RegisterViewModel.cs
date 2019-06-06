using System.ComponentModel.DataAnnotations;
using Destructurama.Attributed;

namespace Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [LogMasked(Text="[ removed for security reasons ]")]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [LogMasked(Text="[ removed for security reasons ]")]
        public string ConfirmPassword { get; set; }
    }
}