using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.ViewModels
{
    public class EditProfileBannerPicViewModel
    {
        [Required(ErrorMessage = "You need to choose an image")]
        public IFormFile? BannerPic { get; set; }
    }
}
