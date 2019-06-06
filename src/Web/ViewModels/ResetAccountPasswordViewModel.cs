using System.ComponentModel.DataAnnotations;
using Destructurama.Attributed;
using Newtonsoft.Json;

namespace Web.ViewModels
{
    public class ResetAccountPasswordViewModel
    {
        [Required]
        [LogMasked(Text="[ removed for security reasons ]")]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Email { get; set; }
    }
}