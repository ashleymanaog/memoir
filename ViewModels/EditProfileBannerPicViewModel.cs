using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.ViewModels
{
    public class EditProfileBannerPicViewModel
    {
        [Required]
        public string DefaultBanner { get; set; }
        
        public byte[]? BannerPic { get; set; }
        
        public IFormFile? NewBannerPic { get; set; }
    }
}