using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThomasianMemoir.ViewModels
{
    public class EditProfileProfilePicViewModel
    {
        [Required]
        public string DefaultAvatar { get; set; }

        public string? ProfilePic { get; set; }
        
        public IFormFile? NewProfilePic { get; set; }
    }
}