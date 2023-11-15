using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.ViewModels
{
    public class EditProfileProfilePicViewModel
    {
        [Required(ErrorMessage = "You need to choose an image")]
        public IFormFile? ProfilePic { get; set; }
    }
}
