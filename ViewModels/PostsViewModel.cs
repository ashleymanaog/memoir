 using ThomasianMemoir.Models;

namespace ThomasianMemoir.ViewModels
{
    public class PostsViewModel
    {
        //Retrieve Posts
        public List<PostWithDetails> PostsWithDetails { get; set; }
        public string DefaultAvatar { get; set; }
        public string UserProfile { get; set; }
        public string YearLevel { get; set; }
        //Create Post
        public string UserId { get; set; }
        public DateTime PostDate { get; set; }
        public string Content { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public string PostType { get; set; }
        public bool IsSensitiveInfo { get; set; }
        public List<IFormFile>? PostMedia { get; set; }
    }
}
