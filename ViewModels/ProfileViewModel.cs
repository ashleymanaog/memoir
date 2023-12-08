using NuGet.DependencyResolver;
using ThomasianMemoir.Models;

namespace ThomasianMemoir.ViewModels
{
    public class ProfileViewModel
    {
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string YearLevel { get; set; }
        public string? ProfileDescription { get; set; }
        public string DefaultAvatar { get; set; }
        public string? ProfilePic { get; set; }
        public string DefaultBanner { get; set; }
        public string? BannerPic { get; set; }
        public List<PostWithDetails> Posts { get; set; }
        public ProfileEditPostViewModel EditPost { get; set; }
        public ProfileViewModel()
        {
            EditPost = new ProfileEditPostViewModel();
        }
    }
}