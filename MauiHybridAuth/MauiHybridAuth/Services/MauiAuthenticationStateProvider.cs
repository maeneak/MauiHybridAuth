using MauiHybridAuth.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Claims;

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
        public AccessTokenInfo? AccessTokenInfo { get; }
        Task<AuthenticationState> GetAuthenticationStateAsync();
        Task LogInAsync(LoginModel loginModel);
        void Logout();
        Task RegisterAsync(string registerUrl); // Add this method to the interface
    }
    /// <summary>
    /// This class manages the authentication state of the user. 
    /// The class handles user login, logout, and token validation, including refreshing tokens when they are close to expiration.
    /// It uses secure storage to save and retrieve tokens, ensuring that users do not need to log in every time.
    /// </summary>
    public class MauiAuthenticationStateProvider : AuthenticationStateProvider, ICustomAuthenticationStateProvider
    {
        //TODO: Place this in AppSettings or Client config file
        private const string AuthenticationType = "Custom authentication";
        private const int TokenExpirationBuffer = 30; //minutes

        public LoginStatus LoginStatus { get; set; } = LoginStatus.None;
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        private AccessTokenInfo? _accessToken;
        public AccessTokenInfo? AccessTokenInfo
        {
            get => _accessToken;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //See if the token stored in SecureStorage is still valid and return the authentications state of the user
            return await CheckTokenAsync();             
        }  

        public void Logout()
        {
            LoginStatus = LoginStatus.None;
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            _accessToken = null;
            TokenStorage.RemoveToken();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }

        public Task LogInAsync(LoginModel loginModel)
        {
            var loginTask = LogInAsyncCore(loginModel);
            NotifyAuthenticationStateChanged(loginTask);

            return loginTask;

            async Task<AuthenticationState> LogInAsyncCore(LoginModel loginModel)
            {
                var user = await LoginWithProviderAsync(loginModel);
                _currentUser = user;

                return new AuthenticationState(_currentUser);
            }
        }
                       
        private async Task<ClaimsPrincipal> LoginWithProviderAsync(LoginModel loginModel)
        {
            ClaimsPrincipal authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
            LoginStatus = LoginStatus.None;

            try
            {
                //Call the Login endpoint and pass the email and password
                var httpClient = HttpClientHelper.GetHttpClient();
                var loginData = new { loginModel.Email, loginModel.Password };
                var response = await httpClient.PostAsJsonAsync(HttpClientHelper.LoginUrl, loginData);

                LoginStatus = response.IsSuccessStatusCode ? LoginStatus.Success : LoginStatus.Failed;

                if (LoginStatus == LoginStatus.Success)
                {
                    // Save token to secure storage so the user doesn't have to login every time
                    var token = await response.Content.ReadAsStringAsync();
                    _accessToken = await TokenStorage.SaveTokenToSecureStorageAsync(token, loginModel.Email);
                                        
                    authenticatedUser = CreateAuthenticatedUser(loginModel.Email);
                }               
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error logging in: {ex}");                
            }

            return authenticatedUser;
        }

        public Task<AuthenticationState> CheckTokenAsync()
        {
            var checkTask = CheckTokenAsyncCore();
            NotifyAuthenticationStateChanged(checkTask);

            return checkTask;

            async Task<AuthenticationState> CheckTokenAsyncCore()
            {
                var user = await CheckTokenValidityAsync();
                _currentUser = user;

                return new AuthenticationState(_currentUser);
            }
        }
        private async Task<ClaimsPrincipal> CheckTokenValidityAsync()
        {
            ClaimsPrincipal authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity());
            LoginStatus = LoginStatus.None;

            try
            {
                _accessToken = await TokenStorage.GetTokenFromSecureStorageAsync();

                if (_accessToken?.LoginToken != null)                
                {                    
                    if (DateTime.UtcNow < _accessToken.TokenExpiration) 
                    {
                        if (DateTime.UtcNow.AddMinutes(TokenExpirationBuffer) >= _accessToken.TokenExpiration)
                        { 
                            //If the token is close to expiration (within 30 minutes), refresh it automatically
                            await RefreshAccessTokenAsync(_accessToken.LoginToken.RefreshToken, _accessToken.Email);
                        }
                        
                        authenticatedUser = CreateAuthenticatedUser(_accessToken.Email);
                        LoginStatus = LoginStatus.Success;
                    }
                }
                if (LoginStatus != LoginStatus.Success)
                {                   
                    Logout();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking token for validity: {ex}");
            }

            return authenticatedUser;
        }

        private async Task RefreshAccessTokenAsync(string refreshToken, string email)
        {
            try
            {
                if (refreshToken != null)
                {
                    //Call the Refresh endpoint and pass the refresh token
                    var httpClient = HttpClientHelper.GetHttpClient();
                    var refreshData = new { refreshToken };
                    var response = await httpClient.PostAsJsonAsync(HttpClientHelper.RefreshUrl, refreshData);
                    if (response.IsSuccessStatusCode)
                    {
                        var token = await response.Content.ReadAsStringAsync();
                        _accessToken = await TokenStorage.SaveTokenToSecureStorageAsync(token, email);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing access token: {ex}");
            }  
        }

        private ClaimsPrincipal CreateAuthenticatedUser(string email)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, email) };  //TODO: Add more claims as needed
            var identity = new ClaimsIdentity(claims, AuthenticationType);
            return new ClaimsPrincipal(identity);
        }

    }
}