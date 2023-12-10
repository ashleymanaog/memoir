using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } // for reset token

        public ResetPasswordViewModel ResetPassword { get; set; } //for chaging password itself

        public LoginViewModel()
        {
            ResetPassword = new ResetPasswordViewModel();
        }
    }
}
