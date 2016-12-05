using System.ComponentModel.DataAnnotations;

namespace SyncthingWeb.Areas.Setup.Models
{
    public class SetupViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Endpoing address is required.")]
        public string SyncthingEndpoint { get; set; }

        [Required(ErrorMessage = "Endpoing API key is required.")]
        public string SyncthingApiKey { get; set; }

        public bool EnableRegistration { get; set; }
    }
}