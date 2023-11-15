using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ThomasianMemoir.Data;
using ThomasianMemoir.Models;
using ThomasianMemoir.ViewModels;

namespace ThomasianMemoir.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _dbContext;
        private DbSet<UserInfo> UserInfo;
        private DbSet<UserPost> UserPost;
        private DbSet<UserPostLikes> UserPostLikes;
        private DbSet<UserPostComments> UserPostComments;
        private DbSet<UserPostMedia> UserPostMedia;

        private readonly UserManager<User> _userManager;

        public ProfileController(AppDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            UserInfo = _dbContext.UserInfo;
            UserPost = _dbContext.UserPost;
            UserPostLikes = _dbContext.UserPostLikes;
            UserPostComments = _dbContext.UserPostComments;
            UserPostMedia = _dbContext.UserPostMedia;
        }

        [HttpGet] /*wla pa posts*/
        public async Task<IActionResult> Profile()
        {
            try
            {
                var currentUser = _userManager.GetUserAsync(User).Result;
                if (currentUser == null)
                {
                    return NotFound();
                }

                var userProfile = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(currentUser.Id));
                if (userProfile == null)
                {
                    return NotFound();
                }

                var viewModel = new ProfileViewModel
                {
                    Email = currentUser.Email,
                    Username = currentUser.UserName,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    YearLevel = userProfile.YearLevel,
                    ProfileDescription = userProfile.ProfileDescription
                    /*Profile and Banner Pic and Posts to add*/
                };
                return View(viewModel);
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", new ErrorViewModel { ErrorMessage = "An unexpected error occurred." });
            }
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            try
            {
                var currentUser = _userManager.GetUserAsync(User).Result;
                if (currentUser == null)
                {
                    return NotFound();
                }

                var userProfile = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(currentUser.Id));
                if (userProfile == null)
                {
                    return NotFound();
                }

                var viewModel = new EditProfileCompositeViewModel
                {
                    PersonalInfo = new EditProfilePersonalInfoViewModel  {
                        FirstName = userProfile.FirstName,
                        LastName = userProfile.LastName,
                        Username = currentUser.UserName,
                        Email = currentUser.Email,
                        YearLevel = userProfile.YearLevel
                    },
                    Password = new EditProfilePasswordViewModel(),
                    ProfileDescription = new EditProfileDescriptionViewModel  {
                        ProfileDescription = userProfile.ProfileDescription
                    },
                    ProfilePic = new EditProfileProfilePicViewModel  {
                        ProfilePic = userProfile.ProfilePic /*not sure how this works*/
                    },
                    BannerPic = new EditProfileBannerPicViewModel  {
                        BannerPic = userProfile.BannerPic /*not sure how this works*/
                    }
                };
                return View(viewModel);
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", new ErrorViewModel { ErrorMessage = "An error occurred while retrieving user information." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("UpdatePersonalInfo")]
        public async Task<IActionResult> UpdatePersonalInfo(EditProfileCompositeViewModel model) {
            TempData["TempProfileDescription"] = model.ProfileDescription.ProfileDescription;
            TempData["IsPostOperation"] = true;
            ModelState.Remove("Password.OldPassword");
            ModelState.Remove("Password.NewPassword");
            ModelState.Remove("Password.ConfirmNewPassword");
            ModelState.Remove("ProfileDescription.ProfileDescription");
            ModelState.Remove("ProfilePic.ProfilePic");
            ModelState.Remove("BannerPic.BannerPic");

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    try
                    {
                        //Update username and email in AspNetUsers table
                        currentUser.UserName = model.PersonalInfo.Username;
                        currentUser.Email = model.PersonalInfo.Email;
                        await _userManager.UpdateAsync(currentUser);

                        var userInfo = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(currentUser.Id));

                        if (userInfo != null)
                        {
                            //Update FirstName, LastName, YearLevel in UserInfo table
                            userInfo.FirstName = model.PersonalInfo.FirstName;
                            userInfo.LastName = model.PersonalInfo.LastName;
                            userInfo.YearLevel = model.PersonalInfo.YearLevel;
                            await _dbContext.SaveChangesAsync();
                        }
                        return RedirectToAction("EditProfile");
                    }
                    catch (Exception e)
                    {
                        foreach (var modelState in ModelState.Values)
                        {
                            foreach (var error in modelState.Errors)
                            {
                                ModelState.AddModelError("updatePersonalInfoErr", e.Message);
                            }
                        }
                    }
                }
            }
            return View("EditProfile", model); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword(EditProfileCompositeViewModel model)
        {
            ModelState.Remove("PersonalInfo.FirstName");
            ModelState.Remove("PersonalInfo.LastName");
            ModelState.Remove("PersonalInfo.Username");
            ModelState.Remove("PersonalInfo.Email");
            ModelState.Remove("PersonalInfo.YearLevel");
            ModelState.Remove("ProfileDescription.ProfileDescription");
            ModelState.Remove("ProfilePic.ProfilePic");
            ModelState.Remove("BannerPic.BannerPic");

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    //Update password in AspNetUsers table
                    var changePasswordResult = await _userManager.ChangePasswordAsync(currentUser, model.Password.OldPassword, model.Password.NewPassword);
                    if (changePasswordResult.Succeeded)
                    {
                        return RedirectToAction("EditProfile");
                    }
                    else
                    {
                        foreach (var error in changePasswordResult.Errors)
                        {
                            ModelState.AddModelError("changePasswordErr", error.Description);
                        }
                    }
                }
            }
            return View("EditProfile", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("UpdateProfileDescription")]
        public async Task<IActionResult> UpdateProfileDescription(EditProfileCompositeViewModel model)
        {
            TempData["TempFirstName"] = model.PersonalInfo.FirstName;
            TempData["TempLastName"] = model.PersonalInfo.LastName;
            TempData["TempUsername"] = model.PersonalInfo.Username;
            TempData["TempEmail"] = model.PersonalInfo.Email;
            TempData["TempyearLevel"] = model.PersonalInfo.YearLevel;
            TempData["IsPostOperation"] = true;
            ModelState.Remove("PersonalInfo.FirstName");
            ModelState.Remove("PersonalInfo.LastName");
            ModelState.Remove("PersonalInfo.Username");
            ModelState.Remove("PersonalInfo.Email");
            ModelState.Remove("PersonalInfo.YearLevel");
            ModelState.Remove("Password.OldPassword");
            ModelState.Remove("Password.NewPassword");
            ModelState.Remove("Password.ConfirmNewPassword");
            ModelState.Remove("ProfilePic.ProfilePic");
            ModelState.Remove("BannerPic.BannerPic");

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    try
                    {
                        var userInfo = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(currentUser.Id));

                        if (userInfo != null)
                        {
                            //Update Profile Description in UserInfo table
                            userInfo.ProfileDescription = model.ProfileDescription.ProfileDescription;
                            await _dbContext.SaveChangesAsync();
                        }
                        return RedirectToAction("EditProfile");
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("updateProfileDescriptionErr", e.Message);
                    }
                }
            }
            return View("EditProfile", model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(EditProfileCompositeViewModel model)
        {
            ModelState.Remove("PersonalInfo.FirstName");
            ModelState.Remove("PersonalInfo.LastName");
            ModelState.Remove("PersonalInfo.Username");
            ModelState.Remove("PersonalInfo.Email");
            ModelState.Remove("PersonalInfo.YearLevel");
            ModelState.Remove("Password.OldPassword");
            ModelState.Remove("Password.NewPassword");
            ModelState.Remove("Password.ConfirmNewPassword");
            ModelState.Remove("ProfilePic.BannerPic");
            return RedirectToAction("EditProfile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("UpdateBanner")]
        public async Task<IActionResult> UpdateBanner([Bind(nameof(EditProfileCompositeViewModel.BannerPic))] EditProfileCompositeViewModel model)
        {
            ModelState.Remove("PersonalInfo.FirstName");
            ModelState.Remove("PersonalInfo.LastName");
            ModelState.Remove("PersonalInfo.Username");
            ModelState.Remove("PersonalInfo.Email");
            ModelState.Remove("PersonalInfo.YearLevel");
            ModelState.Remove("Password.OldPassword");
            ModelState.Remove("Password.NewPassword");
            ModelState.Remove("Password.ConfirmNewPassword");
            ModelState.Remove("ProfilePic.ProfilePic");
            return RedirectToAction("EditProfile");
        }
    }
}
