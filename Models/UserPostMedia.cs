using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.Models
{
    public class UserPostMedia
    {
        [Key]
        public int MediaId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Media { get; set; }
    }
}
