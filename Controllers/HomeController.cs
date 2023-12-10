﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using ThomasianMemoir.Data;
using ThomasianMemoir.Models;
using ThomasianMemoir.Services;
using ThomasianMemoir.ViewModels;

namespace ThomasianMemoir.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly AppDbContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private DbSet<UserInfo> UserInfo;
        //private DbSet<UserPost> UserPost;
        //private DbSet<UserPostLikes> UserPostLikes;
        //private DbSet<UserPostComments> UserPostComments;
        //private DbSet<UserPostMedia> UserPostMedia;

        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly EmailService _emailService;

        public HomeController(ILogger<HomeController> logger, AppDbContext dbContext, SignInManager<User> signInManager, UserManager<User> userManager, IWebHostEnvironment environment, EmailService emailService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _environment = environment;
            _emailService = emailService;
            UserInfo = _dbContext.UserInfo;
            //UserPost = _dbContext.UserPost;
            //UserPostLikes = _dbContext.UserPostLikes;
            //UserPostComments = _dbContext.UserPostComments;
            //UserPostMedia = _dbContext.UserPostMedia;
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
            ModelState.Remove("Email");
            ModelState.Remove("ResetPassword.Email");
            ModelState.Remove("ResetPassword.Token");
            ModelState.Remove("ResetPassword.Password");
            ModelState.Remove("ResetPassword.ConfirmPassword");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginInfo.Username, loginInfo.Password, loginInfo.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Homepage", "Dashboard");
                }
                else
                {
                    ModelState.AddModelError("loginErr", "Username or password is incorrect or user does not exist.");
                }
                return View(loginInfo);
            }
            return View(loginInfo);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(LoginViewModel model)
        {
            ModelState.Remove("Username");
            ModelState.Remove("Password");
            ModelState.Remove("RememberMe");
            ModelState.Remove("ResetPassword.Email");
            ModelState.Remove("ResetPassword.Token");
            ModelState.Remove("ResetPassword.Password");
            ModelState.Remove("ResetPassword.ConfirmPassword");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, token }, protocol: HttpContext.Request.Scheme);
                    await _emailService.SendPasswordResetEmailAsync(model.Email, token, callbackUrl);
                    // Now, send the email with the reset link containing the token
                    // Use your email service or IdentityMessageService to send the email
                    // (Implementation of email sending is not provided here)
                }
                return RedirectToAction("Login");
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(LoginViewModel model)
        {
            ModelState.Remove("Username");
            ModelState.Remove("Password");
            ModelState.Remove("RememberMe");
            ModelState.Remove("Email");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    // Verify the reset token
                    var result = await _userManager.ResetPasswordAsync(user, model.ResetPassword.Token, model.ResetPassword.Password);
                    if (result.Succeeded)
                    {
                        // Password reset successful
                        return RedirectToAction("ResetPasswordConfirmation");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                return RedirectToAction("Login");
            }
            return View(model);
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
                var allowedFileTypes = new List<string> { "image/jpe", "image/jpg", "image/jpeg", "image/gif", "image/png", "image/bmp", "image/ico", "image/svg", "image/tif", "image/tiff", "image/ai", "image/drw", "image/pct", "image/psp", "image/xcf", "image/psd", "image/raw", "image/webp" };

                User newUser = new User();
                newUser.UserName = userEnteredData.Username;
                newUser.Email = userEnteredData.Email;
                
                var result = await _userManager.CreateAsync(newUser, userEnteredData.Password);

                if (result.Succeeded)
                {
                    UserInfo newUserInfo = new UserInfo
                    {
                        UserId = newUser.Id,
                        FirstName = userEnteredData.FirstName,
                        LastName = userEnteredData.LastName,
                        YearLevel = userEnteredData.YearLevel,
                        DefaultAvatar = userEnteredData.DefaultAvatar,
                        DefaultBanner = userEnteredData.DefaultBanner
                    };

                    if (userEnteredData.ProfilePic != null)
                    {
                        var file = userEnteredData.ProfilePic;

                        if (!allowedFileTypes.Contains(file.ContentType))
                        {
                            ModelState.AddModelError("PostMedia", "Invalid file type.");
                            return View(userEnteredData);
                        }

                        var newProfilePicPath = Guid.NewGuid().ToString() +
                            Path.GetExtension(userEnteredData.ProfilePic.FileName);

                        var profilePicPath = Path.Combine(_environment.WebRootPath, "uploads",
                            newProfilePicPath.TrimStart('/'));

                        using (var fileStream = new FileStream(profilePicPath, FileMode.Create))
                        {
                            await userEnteredData.ProfilePic.CopyToAsync(fileStream);
                        }

                        newUserInfo.ProfilePic = newProfilePicPath;
                    }

                    if (userEnteredData.BannerPic != null)
                    {
                        var file = userEnteredData.BannerPic;

                        if (!allowedFileTypes.Contains(file.ContentType))
                        {
                            ModelState.AddModelError("PostMedia", "Invalid file type.");
                            return View(userEnteredData);
                        }

                        var newBannerPicPath = Guid.NewGuid().ToString() +
                            Path.GetExtension(userEnteredData.BannerPic.FileName);

                        var bannerPicPath = Path.Combine(_environment.WebRootPath, "uploads",
                            newBannerPicPath.TrimStart('/'));

                        using (var fileStream = new FileStream(bannerPicPath, FileMode.Create))
                        {
                            await userEnteredData.BannerPic.CopyToAsync(fileStream);
                        }

                        newUserInfo.BannerPic = newBannerPicPath;
                    }

                    _dbContext.UserInfo.Add(newUserInfo);
                    newUser.UserInfo = newUserInfo;

                    await _dbContext.SaveChangesAsync();
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}