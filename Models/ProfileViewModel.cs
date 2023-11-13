using NuGet.DependencyResolver;

namespace ThomasianMemoir.Models
{
    public class ProfileViewModel
    {
        public Users User { get; set; }
        public ICollection<UserPost>? Posts { get; set; }
    }
}