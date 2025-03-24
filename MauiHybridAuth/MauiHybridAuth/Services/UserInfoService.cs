using MauiHybridAuth.Shared.Models;
using MauiHybridAuth.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MauiHybridAuth.Services
{
    public class UserInfoService : IUserInfoService
    {
        public UserInfo? UserInfo { get; set; }

    }
}
