using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThomasianMemoir.Models
{
    public class Users
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
        [EmailAddress(ErrorMessage= "Not a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string YearLevel { get; set; }

        [NotMapped]
        public IFormFile? ProfilePic { get; set; }

        [NotMapped]
        public IFormFile? BannerPic { get; set; }

        public string? ProfileDescription { get; set; }

        [ForeignKey("UserId")]
        public ICollection<UserPost>? Posts { get; set; }
    }
}
