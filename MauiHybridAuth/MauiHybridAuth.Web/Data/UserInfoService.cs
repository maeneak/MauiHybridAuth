using MauiHybridAuth.Shared.Models;
using MauiHybridAuth.Shared.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components.Server.Circuits;
using System.Security.Claims;

namespace MauiHybridAuth.Web.Data
{
    public class UserInfoService : IUserInfoService
    {
        public UserInfo? UserInfo { get; set; }

    }

    internal sealed class UserCircuitHandler(
            AuthenticationStateProvider authenticationStateProvider,
            IUserInfoService userService,
            UserManager<ApplicationUser> userManager)
            : CircuitHandler, IDisposable
    {
        public override Task OnCircuitOpenedAsync(Circuit circuit,
            CancellationToken cancellationToken)
        {
            authenticationStateProvider.AuthenticationStateChanged +=
                AuthenticationChanged;

            return base.OnCircuitOpenedAsync(circuit, cancellationToken);
        }

        private void AuthenticationChanged(Task<AuthenticationState> task)
        {
            _ = UpdateAuthentication(task);

            async Task UpdateAuthentication(Task<AuthenticationState> task)
            {
                try
                {
                    var state = await task;
                    if (state.User?.Identity?.IsAuthenticated == true)
                    {
                        var user = await userManager.GetUserAsync(state.User);
                        userService.UserInfo = new UserInfo
                        {
                            Id = user?.Id,
                            Email = user?.Email,
                            Fullname = user?.Fullname,
                            PreferredName = user?.PreferredName,
                            Initials = user?.Initials,
                            ProfilePicture = user?.ProfilePictureBase64
                        };
                    }
                    else
                    {
                        userService.UserInfo = null;
                    }
                }
                catch
                {
                }
            }
        }

        public override async Task OnConnectionUpAsync(Circuit circuit,
            CancellationToken cancellationToken)
        {
            var state = await authenticationStateProvider.GetAuthenticationStateAsync();
            if (state.User?.Identity?.IsAuthenticated == true)
            {
                var user = await userManager.GetUserAsync(state.User);
                userService.UserInfo = new UserInfo
                {
                    Id = user?.Id,
                    Email = user?.Email,
                    Fullname = user?.Fullname,
                    PreferredName = user?.PreferredName,
                    Initials = user?.Initials,
                    ProfilePicture = user?.ProfilePictureBase64
                };
            }
            else
            {
                userService.UserInfo = null;
            }
        }

        public void Dispose()
        {
            authenticationStateProvider.AuthenticationStateChanged -=
                AuthenticationChanged;
        }
    }
}
