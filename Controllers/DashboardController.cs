using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ThomasianMemoir.Data;
using ThomasianMemoir.Models;
using ThomasianMemoir.ViewModels;

namespace ThomasianMemoir.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly AppDbContext _dbContext;
        private DbSet<UserInfo> Users;
        private DbSet<UserPost> UserPost;
        private DbSet<UserPostLikes> UserPostLikes;
        private DbSet<UserPostComments> UserPostComments;
        private DbSet<UserPostMedia> UserPostMedia;

        private readonly UserManager<User> _userManager;

        public DashboardController(ILogger<DashboardController> logger, AppDbContext dbContext, UserManager<User> userManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            Users = _dbContext.UserInfo;
            UserPost = _dbContext.UserPost;
            UserPostLikes = _dbContext.UserPostLikes;
            UserPostComments = _dbContext.UserPostComments;
            UserPostMedia = _dbContext.UserPostMedia;
        }

        public IActionResult Homepage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Freshmen()
        {
            var currentUser = _userManager.GetUserAsync(User).Result;
            var userProfile = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(currentUser.Id));

            var freshmenPosts = _dbContext.UserPost
                .Where(post => post.PostType == "Freshmen")
                .OrderByDescending(post => post.PostDate)
                .Include(post => post.User)
                .Include(post => post.Likes)
                .Include(post => post.Comments)
                .Include(post => post.Media)
                .ToList()
                .Select(post =>
                {
                    var userInfo = _dbContext.UserInfo
                        .Where(u => u.UserId == post.UserId)
                        .FirstOrDefault();

                    return new PostWithDetails
                    {
                        Post = post,
                        UserId = post.UserId,
                        Username = _userManager.FindByIdAsync(post.UserId).Result.UserName,
                        UserMedia = post.Media,
                        UserLikes = post.Likes,
                        UserComments = post.Comments,
                        ProfilePic = userInfo?.ProfilePic,
                        DefaultAvatar = userInfo?.DefaultAvatar,
                        Liked = HasUserLikedPost(currentUser.Id, post.PostId)
                    };
                })
                .ToList();
            
            var viewModel = new PostsViewModel
            {
                PostsWithDetails = freshmenPosts.ToList(),
                UserProfile = userProfile?.ProfilePic,
                DefaultAvatar = userProfile?.DefaultAvatar
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Freshmen(PostsViewModel model)
        {
            ModelState.Remove("UserId");
            ModelState.Remove("PostType");
            ModelState.Remove("PostsWithDetails");
            ModelState.Remove("UserProfile");
            ModelState.Remove("DefaultAvatar");
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var userProfile = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(currentUser.Id));

                if (currentUser != null)
                {
                    var newPost = new UserPost
                    {
                        UserId = currentUser.Id,
                        User = userProfile,
                        PostDate = DateTime.Now,
                        Content = model.Content,
                        LikesCount = 0,
                        CommentsCount = 0,
                        PostType = "Freshmen",
                        Media = new List<UserPostMedia>()
                    };

                    if (model.PostMedia != null)
                    {
                        for (int i = 0; i < model.PostMedia.Count; i++)
                        {
                            var file = model.PostMedia[i];
                            var mediaType = GetMediaType(file.ContentType);

                            byte[] mediaBytes;
                            using (var memoryStream = new MemoryStream())
                            {
                                await file.CopyToAsync(memoryStream);
                                mediaBytes = memoryStream.ToArray();
                            }

                            var media = new UserPostMedia
                            {
                                Media = mediaBytes,
                                MediaType = mediaType
                            };
                            
                            newPost.Media.Add(media);
                        }
                    }

                    _dbContext.UserPost.Add(newPost);
                    await _dbContext.SaveChangesAsync();

                    return RedirectToAction("Freshmen", "Dashboard");
                }
            }
            return RedirectToAction("Freshmen", "Dashboard");
        }

        public IActionResult Sophomore()
        {
            return View();
        }

        public IActionResult Junior()
        {
            return View();
        }

        public IActionResult Senior()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Post(int postId)
        {
            var post = _dbContext.UserPost
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Commentator)
                .Include(p => p.Media)
                .FirstOrDefault(p => p.PostId == postId);

            if (post == null)
            {
                return NotFound();
            }
            else {
                var userProfilePic = post.Comments.FirstOrDefault()?.Commentator.ProfilePic;
                var userDefaultAvatar = post.Comments.FirstOrDefault()?.Commentator.DefaultAvatar;
            }

            var currentUser = _userManager.GetUserAsync(User).Result;
            var userProfile = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(currentUser.Id));

            var postViewModel = new PostViewModel
            {
                PostWithDetails = new PostWithDetails
                {
                    Post = post,
                    PostId = postId,
                    Username = _userManager.FindByIdAsync(post.UserId).Result.UserName,
                    UserMedia = post.Media,
                    UserLikes = post.Likes,
                    UserComments = post.Comments,
                    ProfilePic = post.User?.ProfilePic,
                    DefaultAvatar = post.User?.DefaultAvatar,
                    Liked = HasUserLikedPost(currentUser.Id, post.PostId)
                },
                UserId = currentUser.Id,
                UserProfile = userProfile?.ProfilePic,
                DefaultAvatar = userProfile?.DefaultAvatar
            };

            return View(postViewModel);
        }

        [HttpGet]
        public IActionResult PostPictures(int postId)
        {
            var post = _dbContext.UserPost
                .Include(p => p.User)
                .Include(p => p.Media)
                .FirstOrDefault(p => p.PostId == postId);

            if (post == null)
            {
                return NotFound();
            }

            var currentUser = _userManager.GetUserAsync(User).Result;

            var postWithDetails = new PostWithDetails
            {
                Post = post,
                PostId = postId,
                Username = _userManager.FindByIdAsync(post.UserId).Result.UserName,
                UserMedia = post.Media,
                ProfilePic = post.User?.ProfilePic,
                DefaultAvatar = post.User?.DefaultAvatar
            };

            return View(postWithDetails);
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

        [HttpPost]
        public async Task<IActionResult> LikePost(int postId)
        {
            try {
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser != null)
                {
                    var existingLike = _dbContext.UserPostLikes
                        .FirstOrDefault(like => like.PostId == postId && like.UserId.Equals(currentUser.Id));

                    if (existingLike == null) //not yet liked
                    {
                        var newLike = new UserPostLikes
                        {
                            PostId = postId,
                            Post = _dbContext.UserPost?.Find(postId),
                            UserId = currentUser.Id
                        };

                        _dbContext.UserPostLikes.Add(newLike);
                        _dbContext.SaveChanges();

                        var post = _dbContext.UserPost?.Find(postId);
                        post.LikesCount += 1;
                        _dbContext.SaveChanges();

                        return Json(new { success = true, liked = true, likesCount = post.LikesCount });
                    }
                    return Json(new { success = true, liked = false });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error liking post: {ex.Message}");
            }
            return Json(new { success = false, error = "User not authenticated" });
        }

        private bool HasUserLikedPost(string userId, int postId)
        {
            return _dbContext.UserPostLikes
                .Any(like => like.PostId == postId && like.UserId.Equals(userId));
        }

        [HttpPost]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);

                if (currentUser != null)
                {
                    var existingLike = _dbContext.UserPostLikes
                        .FirstOrDefault(like => like.PostId == postId && like.UserId.Equals(currentUser.Id));

                    if (existingLike != null) //has liked
                    {
                        _dbContext.UserPostLikes.Remove(existingLike);
                        _dbContext.SaveChanges();

                        var post = _dbContext.UserPost?.Find(postId);
                        post.LikesCount -= 1;
                        _dbContext.SaveChanges();

                        return Json(new { success = true, likesCount = post.LikesCount });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error unliking post: {ex.Message}");
            }
            return Json(new { success = false, error = "User not authenticated" });
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
