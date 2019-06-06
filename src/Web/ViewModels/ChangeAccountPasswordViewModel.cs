using System.ComponentModel.DataAnnotations;
using Destructurama.Attributed;

namespace Web.ViewModels
{
    public class ChangeAccountPasswordViewModel
    {
        [Required]
        [LogMasked(Text="[ removed for security reasons ]")]
        [StringLength(100, MinimumLength = 8)]
        public string CurrentPassword { get; set; }

        [Required]
        [LogMasked(Text="[ removed for security reasons ]")]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; }
    }
}