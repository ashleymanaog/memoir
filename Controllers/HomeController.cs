using Microsoft.AspNetCore.Identity;
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

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            Users = _dbContext.Users;
            //UserPost = _dbContext.UserPost;
            //UserPostLikes = _dbContext.UserPostLikes;
            //UserPostComments = _dbContext.UserPostComments;
            //UserPostMedia = _dbContext.UserPostMedia;

            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginInfo)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginInfo.Username, loginInfo.Password, loginInfo.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Homepage", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("loginErr", "Username or password is incorrect");
                }
                return View(loginInfo);
            }
            return View(loginInfo);
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registration(Users userEnteredData)
        {
            if (ModelState.IsValid)
            {
                /*Users.Add(userEnteredData);
                _dbContext.SaveChangesAsync();
                return RedirectToAction("Login");*/

                User newUser = new User();
                newUser.UserName = userEnteredData.Username;
                newUser.FirstName = userEnteredData.FirstName;
                newUser.LastName = userEnteredData.LastName;
                newUser.Email = userEnteredData.Email;

                var result = await _userManager.CreateAsync(newUser, userEnteredData.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("regErr", error.Description);
                    }
                }
            }
            return View(userEnteredData);
        }
        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}