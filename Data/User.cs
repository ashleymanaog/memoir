using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using ThomasianMemoir.Models;

namespace ThomasianMemoir.Data
{
    public class User : IdentityUser
    {
        public UserInfo UserInfo { get; set; }
        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiry { get; set; }

    }
}
