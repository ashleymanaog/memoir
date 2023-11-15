using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThomasianMemoir.Models
{
    public class UserPost
    {
        [Key]
        public int PostId { get; set; }
        public string UserId { get; set; }
        public UserInfo User { get; set; }
        public DateTime PostDate { get; set; }
        public string Content { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        [ForeignKey("UserId")]
        public List<UserPostLikes> Likes { get; set; }
        [ForeignKey("UserId")]
        public List<UserPostComments> Comments { get; set; }
        [ForeignKey("UserId")]
        public List<UserPostMedia> Media { get; set; }
    }
}
