using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ThomasianMemoir.Data;
using ThomasianMemoir.Models;
using ThomasianMemoir.ViewModels;

namespace ThomasianMemoir.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _dbContext;
        private DbSet<UserInfo> UserInfo;
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
            UserInfo = _dbContext.UserInfo;
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
        public async Task<IActionResult> Registration(RegisterViewModel userEnteredData)
        {
            if (ModelState.IsValid)
            {              
                User newUser = new User();
                newUser.UserName = userEnteredData.Username;
                newUser.Email = userEnteredData.Email;
                
                var result = await _userManager.CreateAsync(newUser, userEnteredData.Password);

                if (result.Succeeded)
                {
                    UserInfo newUserInfo = new UserInfo();
                    newUserInfo.UserId = newUser.Id;
                    newUserInfo.FirstName = userEnteredData.FirstName;
                    newUserInfo.LastName = userEnteredData.LastName;
                    newUserInfo.YearLevel = userEnteredData.YearLevel;
                    newUserInfo.DefaultAvatar = userEnteredData.DefaultAvatar;
                    newUserInfo.DefaultBanner = userEnteredData.DefaultBanner;
                    newUserInfo.ProfilePic = ConvertToByteArray(userEnteredData.ProfilePic);
                    newUserInfo.BannerPic = ConvertToByteArray(userEnteredData.BannerPic);
                    _dbContext.UserInfo.Add(newUserInfo);
                    await _dbContext.SaveChangesAsync();
                    /*Profile and Banner Pic to add*/
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