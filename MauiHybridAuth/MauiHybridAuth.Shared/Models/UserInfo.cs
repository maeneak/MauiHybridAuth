using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiHybridAuth.Shared.Models
{
    public class UserInfo
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Fullname { get; set; }
        public string? PreferredName { get; set; }
        public string? Initials { get; set; }
        public byte[] ProfilePicture { get; set; } = Array.Empty<byte>();
        public string ProfilePictureBase64
        {
            get
            {
                return !ProfilePicture.Any() ? $"data:image/*;base64,{Convert.ToBase64String(ProfilePicture)}" : string.Empty;
            }
        }
    }
}
