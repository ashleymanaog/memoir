using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThomasianMemoir.Data;
using ThomasianMemoir.Models;

namespace ThomasianMemoir.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _dbContext;
        private DbSet<UserInfo> Users;
        private DbSet<UserPost> UserPost;
        private DbSet<UserPostLikes> UserPostLikes;
        private DbSet<UserPostComments> UserPostComments;
        private DbSet<UserPostMedia> UserPostMedia;

        public DashboardController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public IActionResult Freshmen()
        {
            return View();
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

        public IActionResult Post()
        {
            return View();
        }
        public IActionResult PostPictures()
        {
            return View();
        }
    }
}
