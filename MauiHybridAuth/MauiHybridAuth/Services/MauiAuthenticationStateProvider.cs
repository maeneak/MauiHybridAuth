using MauiHybridAuth.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        private string _loginUri = "https://localhost:7157/login";
        private string _refreshUri = "https://localhost:7157/refresh";
        private const string StorageKeyName = "access_token";
        private const string AuthenticationType = "Custom authentication";
        private const int TokenExpirationBuffer = 30; //minutes

        public LoginStatus LoginStatus { get; set; } = LoginStatus.None;
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        private AccessTokenInfo? _accessToken;

        public MauiAuthenticationStateProvider()
        {
#if DEBUG
            //See: https://learn.microsoft.com/dotnet/maui/data-cloud/local-web-services
            //Android Emulator uses 10.0.2.2 to refer to localhost            
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                _loginUri = _loginUri.Replace("localhost", "10.0.2.2");
                _refreshUri = _refreshUri.Replace("localhost", "10.0.2.2");
            }
#endif
        }

        private static HttpClient GetHttpClient()
        {
#if WINDOWS || MACCATALYST
            return new HttpClient();
#else
            return new HttpClient(new HttpsClientHandlerService().GetPlatformMessageHandler()); 
#endif
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
            SecureStorage.Remove(StorageKeyName);
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
                var httpClient = MauiAuthenticationStateProvider.GetHttpClient();
                var loginData = new { loginModel.Email, loginModel.Password };
                var response = await httpClient.PostAsJsonAsync(_loginUri, loginData);

                LoginStatus = response.IsSuccessStatusCode ? LoginStatus.Success : LoginStatus.Failed;

                if (LoginStatus == LoginStatus.Success)
                {
                    // Save token to secure storage so the user doesn't have to login every time
                    var token = await response.Content.ReadAsStringAsync();
                    await SaveTokenToSecureStorageAsync(token, loginModel.Email);
                                        
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
                _accessToken = await GetTokenFromSecureStorageAsync();

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
                    var httpClient = MauiAuthenticationStateProvider.GetHttpClient();
                    var refreshData = new { refreshToken };
                    var response = await httpClient.PostAsJsonAsync(_refreshUri, refreshData);
                    if (response.IsSuccessStatusCode)
                    {
                        var token = await response.Content.ReadAsStringAsync();
                        await SaveTokenToSecureStorageAsync(token, email);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error refreshing access token: {ex}");
            }  
        }

        private async Task<AccessTokenInfo> GetTokenFromSecureStorageAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync(StorageKeyName);
                
                if (!string.IsNullOrEmpty(token)) {             
                    return JsonSerializer.Deserialize<AccessTokenInfo>(token);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to retrieve AccessTokenInfo from SecureStorage." + ex);                
            }
            return null;
        }

        private async Task SaveTokenToSecureStorageAsync(string token, string email)
        {
            try
            {
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(email))
                {
                    var loginToken = JsonSerializer.Deserialize<LoginToken>(token);
                    if (loginToken != null)
                    {
                        DateTime tokenExpiration = DateTime.UtcNow.AddSeconds(loginToken.ExpiresIn);

                        _accessToken = new AccessTokenInfo {
                           LoginToken = loginToken, 
                           Email = email,
                           TokenExpiration = tokenExpiration };

                        await SecureStorage.SetAsync(StorageKeyName, JsonSerializer.Serialize<AccessTokenInfo>(_accessToken));
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to save AccessTokenInfo to SecureStorage." + ex);
                _accessToken = null;
            }
        }

        private ClaimsPrincipal CreateAuthenticatedUser(string email)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, email) };  //TODO: Add more claims as needed
            var identity = new ClaimsIdentity(claims, AuthenticationType);
            return new ClaimsPrincipal(identity);
        }

        private class AccessTokenInfo
        {
            public required string Email { get; set; }
            public required LoginToken LoginToken { get; set; }
            public required DateTime TokenExpiration { get; set; }            
        }

        private class LoginToken
        {
            [JsonPropertyName("tokenType")]
            public required string TokenType { get; set; }

            [JsonPropertyName("accessToken")]
            public required string AccessToken { get; set; }

            [JsonPropertyName("expiresIn")]
            public required int ExpiresIn { get; set; } = 0;

            [JsonPropertyName("refreshToken")]
            public required string RefreshToken { get; set; }            
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