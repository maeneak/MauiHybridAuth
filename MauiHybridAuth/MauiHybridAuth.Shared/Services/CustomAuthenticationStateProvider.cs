using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using MauiHybridAuth.Shared.Models;
using System.Security.Claims;
using System.Diagnostics;
using System.Net.Http.Json;

namespace MauiHybridAuth.Shared.Services
{
    public enum LoginStatus
    {
        None,
        Success,
        Failed
    }
    public interface ICustomAuthenticationStateProvider 
    {
        public LoginStatus LoginStatus { get; set; }
        Task<AuthenticationState> GetAuthenticationStateAsync();
        Task LogInAsync(LoginModel loginModel);
        void Logout();
    }
    public abstract class CustomAuthenticationStateProvider : AuthenticationStateProvider, ICustomAuthenticationStateProvider
    {
        //TODO: Place this in AppSettings or Client config file
        private const string LoginUri = "https://localhost:7157/login";

        public LoginStatus LoginStatus { get; set; } = LoginStatus.None;
        protected ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        
        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(new AuthenticationState(currentUser));

        public virtual Task LogInAsync(LoginModel loginModel)
        {
            var loginTask = LogInAsyncCore(loginModel);
            NotifyAuthenticationStateChanged(loginTask);

            return loginTask;

            async Task<AuthenticationState> LogInAsyncCore(LoginModel loginModel)
            {
                var user = await LoginWithProviderAsync(loginModel);
                currentUser = user;

                return new AuthenticationState(currentUser);
            }
        }
        public virtual void Logout()
        {
            LoginStatus = LoginStatus.None;
            currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(currentUser)));
        }

        //Override this method to implement a different authentication method for the MAUI app or the Blazor app.
        //Currently, this method is used by both the MAUI app and the Blazor app and just calls the login endpoint.
        protected virtual async Task<ClaimsPrincipal> LoginWithProviderAsync(LoginModel loginModel)
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
                    //var token = response.Content.ReadAsStringAsync().Result;
                    var claims = new[] { new Claim(ClaimTypes.Name, loginModel.Email) };
                    var identity = new ClaimsIdentity(claims, "Custom authentication");
                    
                    authenticatedUser = new ClaimsPrincipal(identity);
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

    }
}