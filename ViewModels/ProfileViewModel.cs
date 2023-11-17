﻿using NuGet.DependencyResolver;
using ThomasianMemoir.Models;

namespace ThomasianMemoir.ViewModels
{
    public class ProfileViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string YearLevel { get; set; }
        public string? ProfileDescription { get; set; }
        public string DefaultAvatar { get; set; }
        public byte[]? ProfilePic { get; set; }
        public string DefaultBanner { get; set; }
        public byte[]? BannerPic { get; set; }
        public ICollection<UserPost>? Posts { get; set; }
    }
}