using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.ViewModels
{
    public class EditProfilePersonalInfoViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^\D*$", ErrorMessage = "Number is not allowed")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^\D*$", ErrorMessage = "Number is not allowed")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [EmailAddress(ErrorMessage = "Not a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string YearLevel { get; set; }
    }
}
