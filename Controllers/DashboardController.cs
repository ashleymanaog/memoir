using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using ThomasianMemoir.Data;
using ThomasianMemoir.Models;
using ThomasianMemoir.ViewModels;
using ThomasianMemoir.Extensions;
using System;
using ThomasianMemoir.Services;

namespace ThomasianMemoir.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private readonly ProfanityFilterService profanityFilterService;
        private DbSet<UserInfo> Users;
        private DbSet<UserPost> UserPost;
        private DbSet<UserPostLikes> UserPostLikes;
        private DbSet<UserPostComments> UserPostComments;
        private DbSet<UserPostMedia> UserPostMedia;

        private readonly UserManager<User> _userManager;

        public DashboardController(ILogger<DashboardController> logger, AppDbContext dbContext, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _environment = environment;
            profanityFilterService = new ProfanityFilterService();
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
                DefaultAvatar = userProfile?.DefaultAvatar,
                YearLevel = userProfile?.YearLevel
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
            ModelState.Remove("YearLevel");
            if (ModelState.IsValid)
            {
                var allowedFileTypes = new List<string> { "image/jpe", "image/jpg", "image/jpeg", "image/gif", "image/png", "image/bmp", "image/ico", "image/svg", "image/tif", "image/tiff", "image/ai", "image/drw", "image/pct", "image/psp", "image/xcf", "image/psd", "image/raw", "image/webp", "video/avi", "video/divx", "video/flv", "video/m4v", "video/mkv", "video/mov", "video/mp4", "video/mpeg", "video/mpg", "video/ogm", "video/ogv", "video/ogx", "video/rm", "video/rmvb", "video/smil", "video/webm", "video/wmv", "video/xvid", "video/quicktime" };

                var currentUser = await _userManager.GetUserAsync(User);
                var userProfile = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(currentUser.Id));

                if (currentUser != null)
                {
                    bool containsBadWords = profanityFilterService.ContainsBadWords(model.Content);

                    if (containsBadWords)
                    {
                        ModelState.AddModelError("postContent", "The post contains inappropriate language.");
                        return Json(new { success = false, error = "The post contains inappropriate language." });
                    }

                    var newPost = new UserPost
                    {
                        UserId = currentUser.Id,
                        User = userProfile,
                        PostDate = DateTime.Now,
                        Content = model.Content,
                        LikesCount = 0,
                        CommentsCount = 0,
                        PostType = "Freshmen",
                        IsSensitiveInfo = model.IsSensitiveInfo,
                        Media = new List<UserPostMedia>()
                    };

                    if (model.PostMedia != null)
                    {
                        for (int i = 0; i < model.PostMedia.Count; i++)
                        {
                            var file = model.PostMedia[i];

                            if (!allowedFileTypes.Contains(file.ContentType))
                            {
                                ModelState.AddModelError($"PostMedia[{i}]", "Invalid file type.");
                                return View(model);
                            }

                            var mediaType = GetMediaType(file.ContentType);

                            /*byte[] mediaBytes;
                            using (var memoryStream = new MemoryStream())
                            {
                                await file.CopyToAsync(memoryStream);
                                mediaBytes = memoryStream.ToArray();
                            }*/

                            var media = new UserPostMedia
                            {
                                MediaPath = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName),
                                MediaType = mediaType
                            };

                            var filePath = Path.Combine(_environment.WebRootPath, "uploads", media.MediaPath.TrimStart('/'));
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            newPost.Media.Add(media);
                        }
                    }

                    _dbContext.UserPost.Add(newPost);
                    await _dbContext.SaveChangesAsync();

                    return Json(new { success = true });
                }
            }
            return Json(new { success = false, error = "Invalid model state" });
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
            var currentUser = _userManager.GetUserAsync(User).Result;
            if (currentUser == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
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

            // Fetch root comments with their replies
            var comments = _dbContext.UserPostComments
                .Where(c => c.PostId == postId)
                .Include(c => c.Commentator)
                    .ThenInclude(commentator => commentator.User)
                .Include(c => c.Replies)  // Include replies for each root comment
                    .ThenInclude(reply => reply.Commentator)
                        .ThenInclude(replyCommentator => replyCommentator.User)
                .ToList();

            var userProfilePic = post.Comments.FirstOrDefault()?.Commentator.ProfilePic;
            var userDefaultAvatar = post.Comments.FirstOrDefault()?.Commentator.DefaultAvatar;

            /*var currentUser = _userManager.GetUserAsync(User).Result;*/
            var userProfile = _dbContext.UserInfo.FirstOrDefault(up => up.UserId.Equals(currentUser.Id));

            var postViewModel = new PostViewModel
            {
                PostWithDetails = new PostWithDetails
                {
                    Post = post,
                    PostId = postId,
                    UserId = currentUser.Id,
                    Username = _userManager.FindByIdAsync(post.UserId).Result.UserName,
                    UserMedia = post.Media,
                    UserLikes = post.Likes,
                    UserComments = comments,
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

        [HttpPost]
        public IActionResult AddComment(AddCommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var currentUser = _userManager.GetUserAsync(User).Result;

                    //Add New Comment
                    var newComment = new UserPostComments
                    {
                        PostId = model.PostId,
                        UserId = currentUser.Id,
                        Comment = model.NewComment,
                        Timestamp = DateTime.Now,
                        ParentCommentId = model.ParentCommentId
                    };

                    var commentator = _dbContext.UserInfo.FirstOrDefault(u => u.UserId == newComment.UserId);

                    if (commentator != null)
                    {
                        newComment.Commentator = commentator;
                        _dbContext.UserPostComments.Add(newComment);
                        _dbContext.SaveChanges();

                        //Update CommentsCount & List of Comments
                        var post = _dbContext.UserPost.Include(u => u.Comments).FirstOrDefault(p => p.PostId == model.PostId);
                        
                        if (post != null)
                        {
                            post.CommentsCount += 1;
                            post.Comments.Add(newComment);
                            _dbContext.SaveChanges();
                        }

                        //Adding Parent Comment
                        var parentComment = _dbContext.UserPostComments
                            .Include(c => c.Replies)
                            .FirstOrDefault(c => c.CommentId == model.ParentCommentId);

                        if (parentComment != null)
                        {
                            parentComment.Replies.Add(newComment);
                        }
                        _dbContext.SaveChanges();

                        var newCommentForView = _dbContext.UserPostComments
                            .Include(c => c.Commentator)
                            .Include(c => c.Replies)
                            .FirstOrDefault(c => c.CommentId == newComment.CommentId);
                        string idparentComment = parentComment?.CommentId.ToString() ?? "";

                        return Json(new { success = true, commentHtml = this.RenderToString("_CommentPartialView", newCommentForView), commentsCount = post.CommentsCount, postId = post.PostId, parentCommentId = idparentComment });
                    }
                    else
                    {
                        return Json(new { success = false, error = "Commentator not found" });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error adding comment: {ex.Message}");
                    return Json(new { success = false, error = "Invalid model state" });
                }
            }
            else
            {
                return Json(new { success = false, error = "An error occurred while adding the comment." });
            }
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
                        post.Likes.Add(newLike);
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
