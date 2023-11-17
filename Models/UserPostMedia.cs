﻿using System.ComponentModel.DataAnnotations;

namespace ThomasianMemoir.Models
{
    public class UserPostMedia
    {
        [Key]
        public int MediaId { get; set; }
        public int PostId { get; set; }
        public UserPost Post { get; set; }
        public byte[] Media { get; set; }
        public string MediaType { get; set; }
    }
}
