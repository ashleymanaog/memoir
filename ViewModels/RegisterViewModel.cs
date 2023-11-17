using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ThomasianMemoir.Data;

namespace ThomasianMemoir.ViewModels
{
    public class RegisterViewModel
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^\D*$", ErrorMessage = "Number is not allowed")] //any string except number
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
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string YearLevel { get; set; }

        [Required]
        public string DefaultAvatar { get; set; }

        public IFormFile? ProfilePic { get; set; }

        [Required]
        public string DefaultBanner { get; set; }

        public IFormFile? BannerPic { get; set; }
    }
}
