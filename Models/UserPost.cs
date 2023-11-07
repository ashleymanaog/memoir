using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThomasianMemoir.Models
{
    public class UserPost
    {
        [Key]
        public int PostId { get; set; }
        public int UserId { get; set; }
        public DateTime PostDate { get; set; }
        public string Content { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        [ForeignKey("UserId")]
        public ICollection<UserPostLikes> Likes { get; set; }
        [ForeignKey("UserId")]
        public ICollection<UserPostComments> Comments { get; set; }
        [ForeignKey("UserId")]
        public ICollection<UserPostMedia> Media { get; set; }
    }
}
