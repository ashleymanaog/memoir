using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.ViewModels
{
    public class EditProfileDescriptionViewModel
    {
        [Required(ErrorMessage = "This field is required")]
        public string? ProfileDescription { get; set; }
    }
}
