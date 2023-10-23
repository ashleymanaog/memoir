namespace ThomasianMemoir.Models
{
    public class UserPost
    {
        public int PostId { get; set; }
        public string UserId { get; set; }
        public DateTime PostDate { get; set; }
        public string Content { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
    }
}
