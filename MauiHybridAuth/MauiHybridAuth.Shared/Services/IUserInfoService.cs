using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiHybridAuth.Shared.Services
{
    public interface IUserInfoService
    {
        Task<UserInfo> GetUserInfoAsync();
        Task UpdateProfilePictureAsync(IBrowserFile picture);
    }

    public class UserInfo
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Fullname { get; set; }
        public string? PreferredName { get; set; }
        public string? Initials { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
