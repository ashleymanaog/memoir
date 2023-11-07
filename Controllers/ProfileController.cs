using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThomasianMemoir.Data;
using ThomasianMemoir.Models;

namespace ThomasianMemoir.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _dbContext;
        private DbSet<Users> Users;
        private DbSet<UserPost> UserPost;
        private DbSet<UserPostLikes> UserPostLikes;
        private DbSet<UserPostComments> UserPostComments;
        private DbSet<UserPostMedia> UserPostMedia;

        public ProfileController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            Users = _dbContext.Users;
            UserPost = _dbContext.UserPost;
            UserPostLikes = _dbContext.UserPostLikes;
            UserPostComments = _dbContext.UserPostComments;
            UserPostMedia = _dbContext.UserPostMedia;
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult EditProfile()
        {
            return View();
        }
    }
}
