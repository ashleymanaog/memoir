using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.ViewModels
{
    public class ProfileEditPostViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        public int PostId { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string Content { get; set; }
        public List<IFormFile>? PostMedia { get; set; }
        public List<string?>? EditedMediaIds { get; set; }
        public List<string?>? DeletedMediaIds { get; set; }
    }
}
