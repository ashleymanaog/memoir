using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using ThomasianMemoir.Data;
using ThomasianMemoir.Models;
using ThomasianMemoir.ViewModels;

namespace ThomasianMemoir.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private DbSet<UserInfo> UserInfo;
        private DbSet<UserPost> UserPost;
        private DbSet<UserPostLikes> UserPostLikes;
        private DbSet<UserPostComments> UserPostComments;
        private DbSet<UserPostMedia> UserPostMedia;

        private readonly UserManager<User> _userManager;

        public ProfileController(AppDbContext dbContext, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _environment = environment;
            UserInfo = _dbContext.UserInfo;
            UserPost = _dbContext.UserPost;
            UserPostLikes = _dbContext.UserPostLikes;
            UserPostComments = _dbContext.UserPostComments;
            UserPostMedia = _dbContext.UserPostMedia;
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return NotFound();
                }

                var userProfile = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(currentUser.Id));
                if (userProfile == null)
                {
                    return NotFound();
                }

                var postsWithDetails = _dbContext.UserPost
                    .Where(post => post.UserId == currentUser.Id)
                    .OrderByDescending(post => post.PostDate)
                    .Include(post => post.Likes)
                    .Include(post => post.Media)
                    .ToList()
                    .Select(post =>
                    {
                        return new PostWithDetails
                        {
                            Post = post,
                            PostId = post.PostId,
                            UserMedia = post.Media,
                            UserLikes = post.Likes,
                            Liked = HasUserLikedPost(currentUser.Id, post.PostId)
                        };
                    })
                    .ToList();

                var viewModel = new ProfileViewModel
                {
                    UserId = currentUser.Id,
                    Email = currentUser.Email,
                    Username = currentUser.UserName,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    YearLevel = userProfile.YearLevel,
                    ProfileDescription = userProfile.ProfileDescription,
                    DefaultAvatar = userProfile.DefaultAvatar,
                    DefaultBanner = userProfile.DefaultBanner,
                    ProfilePic = userProfile.ProfilePic,
                    BannerPic = userProfile.BannerPic,
                    Posts = postsWithDetails
                };
                return View(viewModel);
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", new ErrorViewModel { ErrorMessage = "An unexpected error occurred." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Profile(int postId)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PublicProfile(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return RedirectToAction("Error");
                }

                var userProfile = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(user.Id));
                if (userProfile == null)
                {
                    return NotFound();
                }

                var postsWithDetails = _dbContext.UserPost
                    .Where(post => post.UserId == userId)
                    .OrderByDescending(post => post.PostDate)
                    .Include(post => post.Likes)
                    .Include(post => post.Media)
                    .ToList()
                    .Select(post =>
                    {
                        return new PostWithDetails
                        {
                            Post = post,
                            PostId = post.PostId,
                            UserMedia = post.Media,
                            UserLikes = post.Likes,
                            Liked = HasUserLikedPost(userId, post.PostId)
                        };
                    })
                    .ToList();

                var viewModel = new ProfileViewModel
                {
                    UserId = userId,
                    Email = user.Email,
                    Username = user.UserName,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    YearLevel = userProfile.YearLevel,
                    ProfileDescription = userProfile.ProfileDescription,
                    DefaultAvatar = userProfile.DefaultAvatar,
                    DefaultBanner = userProfile.DefaultBanner,
                    ProfilePic = userProfile.ProfilePic,
                    BannerPic = userProfile.BannerPic,
                    Posts = postsWithDetails
                };
                return View(viewModel);
            }
            catch (Exception e)
            {
                return RedirectToAction("Error", new ErrorViewModel { ErrorMessage = "Cannot find Profile." });
            }
        }
        
        private bool HasUserLikedPost(string userId, int postId)
        {
            return _dbContext.UserPostLikes
                .Any(like => like.PostId == postId && like.UserId.Equals(userId));
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
                        DefaultAvatar = userProfile.DefaultAvatar,
                        ProfilePic = userProfile.ProfilePic
                    },
                    BannerPic = new EditProfileBannerPicViewModel  {
                        DefaultBanner = userProfile.DefaultBanner,
                        BannerPic = userProfile.BannerPic
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
            var user = _userManager.GetUserAsync(User).Result;
            var info = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(user.Id));
            TempData["TempProfilePic"] = info.ProfilePic;
            TempData["TempDefaultAvatar"] = info.DefaultAvatar;
            TempData["TempBannerPic"] = info.BannerPic;
            TempData["TempDefaultBanner"] = info.DefaultBanner;
            TempData["IsPostOperation"] = true;
            ModelState.Remove("Password.OldPassword");
            ModelState.Remove("Password.NewPassword");
            ModelState.Remove("Password.ConfirmNewPassword");
            ModelState.Remove("ProfileDescription.ProfileDescription");
            ModelState.Remove("ProfilePic.DefaultAvatar");
            ModelState.Remove("ProfilePic.ProfilePic");
            ModelState.Remove("ProfilePic.NewProfilePic");
            ModelState.Remove("BannerPic.DefaultBanner");
            ModelState.Remove("BannerPic.BannerPic");
            ModelState.Remove("BannerPic.NewBannerPic");

            if (ModelState.IsValid)
            {
                TempData.Remove("TempProfilePic");
                TempData.Remove("TempBannerPic");
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
            TempData["TempFirstName"] = model.PersonalInfo.FirstName;
            TempData["TempLastName"] = model.PersonalInfo.LastName;
            TempData["TempUsername"] = model.PersonalInfo.Username;
            TempData["TempEmail"] = model.PersonalInfo.Email;
            TempData["TempyearLevel"] = model.PersonalInfo.YearLevel;
            var user = _userManager.GetUserAsync(User).Result;
            var info = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(user.Id));
            TempData["TempProfilePic"] = info.ProfilePic;
            TempData["TempDefaultAvatar"] = info.DefaultAvatar;
            TempData["TempBannerPic"] = info.BannerPic;
            TempData["TempDefaultBanner"] = info.DefaultBanner;
            TempData["IsPostOperation"] = true;
            ModelState.Remove("PersonalInfo.FirstName");
            ModelState.Remove("PersonalInfo.LastName");
            ModelState.Remove("PersonalInfo.Username");
            ModelState.Remove("PersonalInfo.Email");
            ModelState.Remove("PersonalInfo.YearLevel");
            ModelState.Remove("ProfileDescription.ProfileDescription");
            ModelState.Remove("ProfilePic.DefaultAvatar");
            ModelState.Remove("ProfilePic.ProfilePic");
            ModelState.Remove("ProfilePic.NewProfilePic");
            ModelState.Remove("BannerPic.DefaultBanner");
            ModelState.Remove("BannerPic.BannerPic");
            ModelState.Remove("BannerPic.NewBannerPic");

            if (ModelState.IsValid)
            {
                TempData.Remove("TempProfilePic");
                TempData.Remove("TempBannerPic");
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
            var user = _userManager.GetUserAsync(User).Result;
            var info = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(user.Id));
            TempData["TempProfilePic"] = info.ProfilePic;
            TempData["TempDefaultAvatar"] = info.DefaultAvatar;
            TempData["TempBannerPic"] = info.BannerPic;
            TempData["TempDefaultBanner"] = info.DefaultBanner;
            TempData["IsPostOperation"] = true;
            ModelState.Remove("PersonalInfo.FirstName");
            ModelState.Remove("PersonalInfo.LastName");
            ModelState.Remove("PersonalInfo.Username");
            ModelState.Remove("PersonalInfo.Email");
            ModelState.Remove("PersonalInfo.YearLevel");
            ModelState.Remove("Password.OldPassword");
            ModelState.Remove("Password.NewPassword");
            ModelState.Remove("Password.ConfirmNewPassword");
            ModelState.Remove("ProfilePic.DefaultAvatar");
            ModelState.Remove("ProfilePic.ProfilePic");
            ModelState.Remove("ProfilePic.NewProfilePic");
            ModelState.Remove("BannerPic.DefaultBanner");
            ModelState.Remove("BannerPic.BannerPic");
            ModelState.Remove("BannerPic.NewBannerPic");

            if (ModelState.IsValid)
            {
                TempData.Remove("TempProfilePic");
                TempData.Remove("TempBannerPic");
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
            ModelState.Remove("ProfilePic.ProfilePic");
            ModelState.Remove("BannerPic.DefaultBanner");
            ModelState.Remove("BannerPic.BannerPic");
            ModelState.Remove("BannerPic.NewBannerPic");
            ModelState.Remove("ProfileDescription.ProfileDescription");

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    try
                    {
                        var allowedFileTypes = new List<string> { "image/jpe", "image/jpg", "image/jpeg", "image/gif", "image/png", "image/bmp", "image/ico", "image/svg", "image/tif", "image/tiff", "image/ai", "image/drw", "image/pct", "image/psp", "image/xcf", "image/psd", "image/raw", "image/webp" };

                        var userInfo = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(currentUser.Id));

                        if (userInfo != null)
                        {
                            userInfo.DefaultAvatar = model.ProfilePic.DefaultAvatar;

                            if (model.ProfilePic.NewProfilePic != null)
                            {
                                var file = model.ProfilePic.NewProfilePic;

                                if (!allowedFileTypes.Contains(file.ContentType))
                                {
                                    ModelState.AddModelError("PostMedia", "Invalid file type.");
                                    return View(model);
                                }

                                var newProfilePicPath = Guid.NewGuid().ToString() +
                                    Path.GetExtension(model.ProfilePic.NewProfilePic.FileName);
                                var filePath = Path.Combine(_environment.WebRootPath, "uploads",
                                    newProfilePicPath.TrimStart('/'));

                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await model.ProfilePic.NewProfilePic.CopyToAsync(fileStream);
                                }

                                userInfo.ProfilePic = newProfilePicPath;
                            }
                            else
                            {
                                userInfo.ProfilePic = null;
                            }
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
            ModelState.Remove("ProfilePic.DefaultAvatar");
            ModelState.Remove("ProfilePic.ProfilePic");
            ModelState.Remove("ProfilePic.NewProfilePic");
            ModelState.Remove("BannerPic.BannerPic");
            ModelState.Remove("ProfileDescription.ProfileDescription");

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    try
                    {
                        var allowedFileTypes = new List<string> { "image/jpe", "image/jpg", "image/jpeg", "image/gif", "image/png", "image/bmp", "image/ico", "image/svg", "image/tif", "image/tiff", "image/ai", "image/drw", "image/pct", "image/psp", "image/xcf", "image/psd", "image/raw", "image/webp" };

                        var userInfo = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(currentUser.Id));

                        if (userInfo != null)
                        {
                            userInfo.DefaultBanner = model.BannerPic.DefaultBanner;

                            if (model.BannerPic.NewBannerPic != null)
                            {
                                var file = model.BannerPic.NewBannerPic;

                                if (!allowedFileTypes.Contains(file.ContentType))
                                {
                                    ModelState.AddModelError("PostMedia", "Invalid file type.");
                                    return View(model);
                                }

                                var newBannerPicPath = Guid.NewGuid().ToString() +
                                    Path.GetExtension(model.BannerPic.NewBannerPic.FileName);
                                var filePath = Path.Combine(_environment.WebRootPath, "uploads",
                                    newBannerPicPath.TrimStart('/'));

                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await model.BannerPic.NewBannerPic.CopyToAsync(fileStream);
                                }

                                userInfo.BannerPic = newBannerPicPath;
                            }
                            else
                            {
                                userInfo.BannerPic = null;
                            }
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
        [ActionName("ConfirmDelete")]
        public async Task<IActionResult> ConfirmDelete(int postId)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser != null)
                {
                    try
                    {
                        var post = _dbContext.UserPost.FirstOrDefault(p => p.PostId == postId);
                        if (post == null)
                        {
                            return NotFound();
                        }
                        _dbContext.UserPost.Remove(post);
                        _dbContext.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("removePostErr", e.Message);
                    }
                }
            }
            return RedirectToAction("Profile");
        }

        private byte[] ConvertToByteArray(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }
            using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                return stream.ToArray();
            }
        }
    }
}
