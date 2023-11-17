using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ThomasianMemoir.Models;

namespace ThomasianMemoir.ViewModels
{
    public class CreatePostViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public DateTime PostDate { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
