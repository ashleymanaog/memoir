namespace ThomasianMemoir.ViewModels
{
    public class PostViewModel
    {
        public PostWithDetails PostWithDetails { get; set; }
        public string DefaultAvatar { get; set; }
        public string UserProfile { get; set; }
        public string UserId { get; internal set; }
    }
}
