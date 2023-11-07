using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThomasianMemoir.Models
{
    public enum YearLevel
    {
        FirstYear, SecondYear, ThirdYear, FourthYear, FifthYear
    }
    public class Users
    {
        [Key]
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public YearLevel YearLevel { get; set; }
        public string ProfilePic { get; set; }
        public string BannerPic { get; set; }
        public string? ProfileDescription { get; set; }
        [ForeignKey("UserId")]
        public ICollection<UserPost> Posts { get; set; }
    }
}
