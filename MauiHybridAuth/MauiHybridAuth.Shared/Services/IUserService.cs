using MauiHybridAuth.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiHybridAuth.Shared.Services
{
    public interface IUserService
    {
        public UserInfo UserInfo { get; }
        public Task SetProfilePicture(byte[] picture);
    }
}
