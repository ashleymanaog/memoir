using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThomasianMemoir.Data;

namespace ThomasianMemoir.Models
{
    public class UserInfo
    {
        [Key]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^\D*$", ErrorMessage = "Number is not allowed")] //any string except number
        public string FirstName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^\D*$", ErrorMessage = "Number is not allowed")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "This field is required")]
        public string YearLevel { get; set; }

        [Required]
        public string DefaultAvatar { get; set; }

        /*[Column(TypeName = "VARBINARY(MAX)")]*/
        public string? ProfilePic { get; set; }

        [Required]
        public string DefaultBanner { get; set; }

        /*[Column(TypeName = "VARBINARY(MAX)")]*/
        public string? BannerPic { get; set; }

        public string? ProfileDescription { get; set; }

        [ForeignKey("UserId")]
        public List<UserPost>? Posts { get; set; }
    }
}
