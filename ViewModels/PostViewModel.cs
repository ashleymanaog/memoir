namespace ThomasianMemoir.ViewModels
{
    public class PostViewModel
    {
        public PostWithDetails PostWithDetails { get; set; }
        public string DefaultAvatar { get; set; }
        public byte[] UserProfile { get; set; }
        public string UserId { get; internal set; }
    }
}
