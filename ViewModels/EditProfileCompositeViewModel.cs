using System.ComponentModel.DataAnnotations;
using ThomasianMemoir.Models;

namespace ThomasianMemoir.ViewModels
{
    public class EditProfileCompositeViewModel
    {
        public EditProfilePersonalInfoViewModel PersonalInfo { get; set; }
        public EditProfilePasswordViewModel Password { get; set; }
        public EditProfileDescriptionViewModel ProfileDescription { get; set; }
        public EditProfileProfilePicViewModel ProfilePic { get; set; }
        public EditProfileBannerPicViewModel BannerPic { get; set; }

        public EditProfileCompositeViewModel()
        {
            PersonalInfo = new EditProfilePersonalInfoViewModel();
            Password = new EditProfilePasswordViewModel();
            ProfileDescription = new EditProfileDescriptionViewModel();
            ProfilePic = new EditProfileProfilePicViewModel();
            BannerPic = new EditProfileBannerPicViewModel();
        }
    }
}