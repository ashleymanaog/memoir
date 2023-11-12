using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ThomasianMemoir.Data;
using ThomasianMemoir.Models;

namespace ThomasianMemoir.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _dbContext;
        private DbSet<Users> Users;
        //private DbSet<UserPost> UserPost;
        //private DbSet<UserPostLikes> UserPostLikes;
        //private DbSet<UserPostComments> UserPostComments;
        //private DbSet<UserPostMedia> UserPostMedia;
                
        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
            Users = _dbContext.Users;
            //UserPost = _dbContext.UserPost;
            //UserPostLikes = _dbContext.UserPostLikes;
            //UserPostComments = _dbContext.UserPostComments;
            //UserPostMedia = _dbContext.UserPostMedia;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(Users newUser)
        {
            if (ModelState.IsValid)
            {
                Users.Add(newUser);
                _dbContext.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return RedirectToAction("Registration");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}