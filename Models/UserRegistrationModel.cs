using System;
using System.ComponentModel.DataAnnotations;

namespace WALEED_TMS.Models
{
    public class UserRegistrationModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [EmailAddress]
        [Display(Name = "Contact Email Address")]
        public string ContactEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Where did you hear about us?")]
        public string HearAboutUs { get; set; }

        [Display(Name = "Preferred Way to Contact")]
        public string PreferredContact { get; set; }
    }
}
