using System.ComponentModel.DataAnnotations;
using Destructurama.Attributed;

namespace Web.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [LogMasked(Text="[ removed for security reasons ]")]
        public string Password { get; set; }
    }
}