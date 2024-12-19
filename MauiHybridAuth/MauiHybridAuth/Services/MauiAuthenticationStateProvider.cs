using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using MauiHybridAuth.Models;
using System.Security.Claims;
using System.Diagnostics;
using System.Net.Http.Json;

namespace MauiHybridAuth.Services
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
    public class MauiAuthenticationStateProvider : AuthenticationStateProvider, ICustomAuthenticationStateProvider
    {
        //TODO: Place this in AppSettings or Client config file
        protected string LoginUri { get; set; } = "https://localhost:7157/login";

        public LoginStatus LoginStatus { get; set; } = LoginStatus.None;
        protected ClaimsPrincipal currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public MauiAuthenticationStateProvider()
        {
            //See: https://learn.microsoft.com/dotnet/maui/data-cloud/local-web-services
            //Android Emulator uses 10.0.2.2 to refer to localhost            
            LoginUri =
                DeviceInfo.Platform == DevicePlatform.Android ? LoginUri.Replace("localhost", "10.0.2.2") : LoginUri;
        }

        private HttpClient GetHttpClient()
        {
#if WINDOWS || MACCATALYST
            return new HttpClient();
#else
            return new HttpClient(new HttpsClientHandlerService().GetPlatformMessageHandler()); 
#endif
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(new AuthenticationState(currentUser));

        public Task LogInAsync(LoginModel loginModel)
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
        public void Logout()
        {
            LoginStatus = LoginStatus.None;
            currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(currentUser)));
        }
                  
        private async Task<ClaimsPrincipal> LoginWithProviderAsync(LoginModel loginModel)
        {
            ClaimsPrincipal authenticatedUser;
            LoginStatus = LoginStatus.None;

            try
            {
                var httpClient = GetHttpClient();
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

    public class HttpsClientHandlerService
    {
        public HttpMessageHandler GetPlatformMessageHandler()
        {
#if ANDROID
            var handler = new Xamarin.Android.Net.AndroidMessageHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
            return handler;
#elif IOS
        var handler = new NSUrlSessionHandler
        {
            TrustOverrideForUrl = IsHttpsLocalhost
        };
        return handler;
#else
            throw new PlatformNotSupportedException("Only Android and iOS supported.");
#endif
        }

#if IOS
    public bool IsHttpsLocalhost(NSUrlSessionHandler sender, string url, Security.SecTrust trust)
    {
        return url.StartsWith("https://localhost");
    }
#endif
    }

}