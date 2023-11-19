using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.Models
{
    public class UserPostComments
    {
        [Key]
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public UserPost Post { get; set; }
        public string UserId { get; set; }
        public UserInfo Commentator { get; set; }
        public string Comment { get; set; }
        public int? ParentCommentId { get; set; }
        public UserPostComments? ParentComment { get; set; }
        public List<UserPostComments>? Replies { get; set; }
        public DateTime Timestamp { get; set; }
    }
}