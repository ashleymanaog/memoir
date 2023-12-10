using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string VerificationCode { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirm password must match.")]
        public string ConfirmPassword { get; set; }
    }
}
