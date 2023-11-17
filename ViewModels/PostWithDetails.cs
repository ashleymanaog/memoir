using ThomasianMemoir.Models;

namespace ThomasianMemoir.ViewModels
{
    public class PostWithDetails
    {
        //Post Details
        public UserPost Post { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string DefaultAvatar { get; set; }
        public byte[] ProfilePic { get; set; }
        public bool Liked { get; set; }
        public List<UserPostMedia> UserMedia { get; set; }
        public List<UserPostLikes> UserLikes { get; set; }
        public List<UserPostComments> UserComments { get; set; }
    }
}
