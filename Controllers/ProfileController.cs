using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Xml.Linq;
using ThomasianMemoir.Data;
using ThomasianMemoir.Extensions;
using ThomasianMemoir.Models;
using ThomasianMemoir.ViewModels;

namespace ThomasianMemoir.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private DbSet<UserInfo> UserInfo;
        private DbSet<UserPost> UserPost;
        private DbSet<UserPostLikes> UserPostLikes;
        private DbSet<UserPostComments> UserPostComments;
        private DbSet<UserPostMedia> UserPostMedia;

        private readonly UserManager<User> _userManager;

        public ProfileController(ILogger<DashboardController> logger, AppDbContext dbContext, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _logger = logger;
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
                                    return RedirectToAction("EditProfile");
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
                        return RedirectToAction("EditProfile");
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
                                    return RedirectToAction("EditProfile");
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
                        return RedirectToAction("EditProfile");
                    }
                }
            }
            return View("EditProfile", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser != null)
                {
                    using (var transaction = _dbContext.Database.BeginTransaction()) {
                        try
                        {
                            var post = _dbContext.UserPost
                            .FirstOrDefault(p => p.PostId == postId && p.UserId.Equals(currentUser.Id));

                            if (post != null)
                            {
                                // Remove post likes
                                var postLikes = _dbContext.UserPostLikes
                                    .Where(like => like.PostId == postId)
                                    .ToList();
                                _dbContext.UserPostLikes.RemoveRange(postLikes);

                                // Remove post comments
                                var postComments = _dbContext.UserPostComments
                                    .Where(comment => comment.PostId == postId)
                                    .ToList();
                                _dbContext.UserPostComments.RemoveRange(postComments);

                                // Remove post media
                                var postMedia = _dbContext.UserPostMedia
                                    .Where(media => media.PostId == postId)
                                    .ToList();
                                foreach (var media in postMedia)
                                {
                                    // Remove media files from server
                                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", media.MediaPath.TrimStart('/'));
                                    if (System.IO.File.Exists(filePath))
                                    {
                                        System.IO.File.Delete(filePath);
                                    }
                                }
                                _dbContext.UserPostMedia.RemoveRange(postMedia);

                                // Remove post
                                _dbContext.UserPost.Remove(post);
                                transaction.Commit();
                                await _dbContext.SaveChangesAsync();

                                return RedirectToAction("Profile");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error deleting post: {ex.Message}");
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting post: {ex.Message}");
            }
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("EditPost")]
        public async Task<IActionResult> EditPost([FromForm] ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var info = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(user.Id));
            TempData["TempFirstName"] = info.FirstName;
            TempData["TempLastName"] = info.LastName;
            TempData["TempUsername"] = user.UserName;
            TempData["TempEmail"] = user.Email;
            TempData["TempYearLevel"] = info.YearLevel;
            TempData["TempProfileDescription"] = info.ProfileDescription;
            TempData["TempProfilePic"] = info.ProfilePic;
            TempData["TempDefaultAvatar"] = info.DefaultAvatar;
            TempData["TempBannerPic"] = info.BannerPic;
            TempData["TempDefaultBanner"] = info.DefaultBanner;
            TempData["IsPostOperation"] = true;
                        
            ModelState.Remove("FirstName");
            ModelState.Remove("LastName");
            ModelState.Remove("Username");
            ModelState.Remove("Email");
            ModelState.Remove("Password");
            ModelState.Remove("YearLevel");
            ModelState.Remove("ProfileDescription");
            ModelState.Remove("DefaultAvatar");
            ModelState.Remove("ProfilePic");
            ModelState.Remove("DefaultBanner");
            ModelState.Remove("BannerPic");
            ModelState.Remove("Posts");

            //using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                if (!ModelState.IsValid)
                {
                    var postsWithDetails = _dbContext.UserPost
                    .Where(post => post.UserId == user.Id)
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
                            UserMedia = post.Media ?? new List<UserPostMedia>(),
                            UserLikes = post.Likes,
                            Liked = HasUserLikedPost(user.Id, post.PostId)
                        };
                    })
                    .ToList();
                    if (postsWithDetails != null && postsWithDetails.Any())
                    {
                        var settings = new JsonSerializerSettings
                        { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
                        TempData["SerializedPosts"] = JsonConvert.SerializeObject(postsWithDetails, settings);
                    }
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors.OfType<ModelError>())
                        .ToList();
                    ModelState.AddModelError("editPost", errors.ToString());
                    return Json(new { success = false, error = "Invalid model state" });
                }

                var currentUser = _userManager.GetUserAsync(User).Result;
                //null here may problem
                var userProfile = _dbContext.UserInfo
                    .Include(up => up.Posts)
                        .ThenInclude(post => post.Media)
                    .FirstOrDefault(up => up.UserId.Equals(currentUser.Id));

                if (currentUser != null && userProfile != null)
                {
                    // Find post to update
                    var postToUpdate = _dbContext.UserPost.FirstOrDefault(p => p.UserId == userProfile.UserId && p.PostId == model.EditPost.PostId);

                    var allowedFileTypes = new List<string> { "image/jpe", "image/jpg", "image/jpeg", "image/gif", "image/png", "image/bmp", "image/ico", "image/svg", "image/tif", "image/tiff", "image/ai", "image/drw", "image/pct", "image/psp", "image/xcf", "image/psd", "image/raw", "image/webp", "video/avi", "video/divx", "video/flv", "video/m4v", "video/mkv", "video/mov", "video/mp4", "video/mpeg", "video/mpg", "video/ogm", "video/ogv", "video/ogx", "video/rm", "video/rmvb", "video/smil", "video/webm", "video/wmv", "video/xvid", "video/quicktime" };

                    if (postToUpdate != null)
                    {
                        // UPDATE POST CONTENT
                        postToUpdate.Content = model.EditPost.Content;
                        postToUpdate.IsSensitiveInfo = model.EditPost.IsSensitiveInfo;

                        // HANDLE EDITED MEDIA
                        if (model.EditPost.EditedMediaIds != null && model.EditPost.EditedMediaIds.Count > 0)
                        {
                            List<string?> editedMediaIdsModel = model.EditPost.EditedMediaIds;

                            List<string> editedMediaIdStrings = new List<string>();

                            foreach (string? s in editedMediaIdsModel)
                            {
                                if (!string.IsNullOrEmpty(s))
                                {
                                    editedMediaIdStrings
                                        .AddRange(s.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(part => part.Trim()));
                                }
                            }

                            // List to store valid indexes
                            var validIndexes = new List<int>();

                            for (int i = 0; i < editedMediaIdStrings.Count; i++)
                            {
                                // Check if the media is edited (editedMediaId is not 0 and not null)
                                if (editedMediaIdStrings[i] != "0")
                                {
                                    if (int.TryParse(editedMediaIdStrings[i], out int editedMediaId))
                                    {
                                        // Find the corresponding UserPostMedia entity in your database
                                        var editedMedia = userProfile.Posts
                                            .Where(p => p.Media != null)
                                            .SelectMany(p => p.Media)
                                            .FirstOrDefault(m => m.MediaId == editedMediaId);

                                        Debug.WriteLine($"Edited Media: {editedMedia}");

                                        // Check if the editedMedia is found
                                        if (editedMedia != null)
                                        {
                                            validIndexes.Add(i);

                                            // Update the media path and media type
                                            var file = model.EditPost.PostMedia[i];
                                            Debug.WriteLine($"File: {file}");

                                            if (!allowedFileTypes.Contains(file.ContentType))
                                            {
                                                ModelState.AddModelError($"PostMedia[{i}]", "Invalid file type.");
                                                return View("Profile", model);
                                            }

                                            editedMedia.MediaPath = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                            editedMedia.MediaType = GetMediaType(file.ContentType);

                                            var filePath = Path.Combine(_environment.WebRootPath, "uploads", editedMedia.MediaPath.TrimStart('/'));
                                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                                            {
                                                await file.CopyToAsync(fileStream);
                                            }

                                            await _dbContext.SaveChangesAsync();
                                        }
                                    }
                                }
                            }
                        }

                        // HANDLE NEW MEDIA
                        if (model.EditPost.EditedMediaIds != null && model.EditPost.EditedMediaIds.Count > 0)
                        {
                            List<string?> editedMediaIdsModel = model.EditPost.EditedMediaIds;

                            List<string> editedMediaIdStrings = new List<string>();

                            foreach (string? s in editedMediaIdsModel)
                            {
                                if (!string.IsNullOrEmpty(s))
                                {
                                    editedMediaIdStrings
                                        .AddRange(s.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(part => part.Trim()));
                                }
                            }

                            // Get indexes of editedMediaIdsArray with mediaId of "0"
                            var mediaIdZeroIndexes = editedMediaIdStrings
                                .Select((value, index) => (value == "0") ? index : -1)
                                .Where(index => index != -1)
                                .ToList();

                            foreach (var index in mediaIdZeroIndexes)
                            {
                                // Check if the index is within the range of model.PostMedia
                                if (index >= 0 && index < model.EditPost.PostMedia.Count)
                                {
                                    var newMediaFile = model.EditPost.PostMedia[index];

                                    if (!allowedFileTypes.Contains(newMediaFile.ContentType))
                                    {
                                        ModelState.AddModelError($"PostMedia[{index}]", "Invalid file type.");
                                        return View(model);
                                    }

                                    var mediaType = GetMediaType(newMediaFile.ContentType);

                                    var media = new UserPostMedia
                                    {
                                        PostId = model.EditPost.PostId,
                                        MediaPath = Guid.NewGuid().ToString() + Path.GetExtension(newMediaFile.FileName),
                                        MediaType = mediaType
                                    };

                                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", media.MediaPath.TrimStart('/'));
                                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                                    {
                                        await newMediaFile.CopyToAsync(fileStream);
                                    }

                                    _dbContext.UserPostMedia.Add(media);
                                }
                            }
                        }

                        // HANDLE DELETED MEDIA
                        if (model.EditPost.DeletedMediaIds != null && model.EditPost.DeletedMediaIds.Count > 0)
                        {
                            List<string?> deletedMediaIdsModel = model.EditPost.DeletedMediaIds;

                            List<string> deletedMediaIdStrings = new List<string>();

                            foreach (string? s in deletedMediaIdsModel)
                            {
                                if (!string.IsNullOrEmpty(s))
                                {
                                    deletedMediaIdStrings
                                        .AddRange(s.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                        .Select(part => part.Trim()));
                                }
                            }

                            for (int i = 0; i < deletedMediaIdStrings.Count; i++)
                            {
                                if (deletedMediaIdStrings[i] != "0") // check not null/empty
                                {
                                    if (int.TryParse(deletedMediaIdStrings[i], out int deletedMediaId))
                                    {
                                        var mediaToDelete = _dbContext.UserPostMedia.FirstOrDefault(m => m.MediaId == deletedMediaId);
                                        if (mediaToDelete != null)
                                        {
                                            _dbContext.UserPostMedia.Remove(mediaToDelete);
                                        }
                                    }
                                }
                            }
                        }
                        _dbContext.Entry(postToUpdate).State = EntityState.Modified;
                        int savedChanges = await _dbContext.SaveChangesAsync();
                        //transaction.Commit();
                        await _dbContext.SaveChangesAsync();
                        //return RedirectToAction("Profile", "Profile");
                        return Json(new { success = true });
                    }
                    else
                    {
                        ModelState.AddModelError("editPost", "Post not found.");
                        return Json(new { success = false, error = "Post not found." });
                    }
                }
                return Json(new { success = false, error = "User not found." });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("editPost", ex.Message);
                _logger.LogError($"Error editing post: {ex.Message}\nStackTrace: {ex.StackTrace}");
                // Access the inner exception for more details
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    Debug.WriteLine(innerException.Message);
                    innerException = innerException.InnerException;
                }
                //transaction.Rollback();
                // Handle exception, log, and redirect as needed
                return Json(new { success = false, error = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)) });
            }
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

        private string GetMediaType(string contentType)
        {
            switch (contentType)
            {
                case var _ when contentType.StartsWith("image/"):
                    return "Image";
                case var _ when contentType.StartsWith("video/"):
                    return "Video";
                default:
                    return "Unknown";
            }
        }
    }
}
