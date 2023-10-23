namespace ThomasianMemoir.Models
{
    public enum YearLevel
    {
        FirstYear, SecondYear, ThirdYear, FourthYear, FifthYear
    }
    public class Users
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public YearLevel YearLevel { get; set; }
        public string ProfilePic { get; set; }
    }
}
