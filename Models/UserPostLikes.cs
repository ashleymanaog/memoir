using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.Models
{
    public class UserPostLikes
    {
        [Key]
        public int LikeId { get; set; }
        public int PostId { get; set; }
        public UserPost Post { get; set; }
        public int UserId { get; set; }
    }
}
