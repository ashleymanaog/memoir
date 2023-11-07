using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.Models
{
    public class UserPostComments
    {
        [Key]
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Comment { get; set; }
    }
}
