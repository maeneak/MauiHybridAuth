using Microsoft.AspNetCore.Components.Authorization;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Claims;
using MauiHybridAuth.Shared.Models;
using MauiHybridAuth.Shared.Services;

namespace MauiHybridAuth.Services
{
    public class MauiAuthenticationStateProvider : CustomAuthenticationStateProvider, ICustomAuthenticationStateProvider
    {
        private const string LoginUri = "https://localhost:7157/login";

        protected override async Task<ClaimsPrincipal> LoginWithProviderAsync(LoginModel loginModel)
        {
            ClaimsPrincipal authenticatedUser;
            LoginStatus = LoginStatus.None;

            try
            {
                var httpClient = new HttpClient();
                var loginData = new { loginModel.Email, loginModel.Password };

                var response = await httpClient.PostAsJsonAsync(LoginUri, loginData);
                LoginStatus = response.IsSuccessStatusCode ? LoginStatus.Success : LoginStatus.Failed;

                if (LoginStatus == LoginStatus.Success)
                {
                    var claims = new[] { new Claim(ClaimTypes.Name, loginModel.Email) };
                    var identity = new ClaimsIdentity(claims, "Custom authentication");
                    authenticatedUser = new ClaimsPrincipal(identity);

                    //token = response.Content.ReadAsStringAsync().Result;
                    //WriteTokenAsync(token);
                }
                else
                    authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error logging in: {ex.ToString()}");
                authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
            }

            return authenticatedUser;
        }
        public override void Logout()
        {
            base.Logout();
            //RemoveTokenAsync();            
        }

        //TODO - Decide if we really want to store tokens/logins in SecureStorage
        //private async Task<string> ReadTokenAsync()
        //{
        //    //TODO Try/Catch
        //    return await SecureStorage.GetAsync("accounttoken");
        //}
        //private async void WriteTokenAsync(string token)
        //{
        //    //TODO Try/Catch
        //    await SecureStorage.SetAsync("accounttoken", token);
        //}
        //private void RemoveTokenAsync()
        //{
        //    //TODO Try/Catch
        //    SecureStorage.Remove("accounttoken");
        //}
    }
}


