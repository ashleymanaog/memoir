using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.ViewModels
{
    public class EditProfilePasswordViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [Compare("NewPassword", ErrorMessage = "New password and confirm new password must match")]
        public string ConfirmNewPassword { get; set; }
    }
}
