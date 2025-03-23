using MauiHybridAuth.Shared.Models;
using MauiHybridAuth.Shared.Services;
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
    public class UserInfoService : IUserInfoService, IDisposable
    {
        private readonly MauiAuthenticationStateProvider stateProvider;
        public UserInfoService(MauiAuthenticationStateProvider StateProvider) {
            stateProvider = StateProvider;
            stateProvider.AuthenticationStateChanged += AuthenticationStateChanged;
        }
        public UserInfo? UserInfo { get; set; }

        private async void AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            await RefreshUserInfoAsync();
        }

        public async Task RefreshUserInfoAsync()
        {
            UserInfo? userInfo = new UserInfo();
            try
            {
                var httpClient = HttpClientHelper.GetHttpClient();
                var userInfoUrl = HttpClientHelper.UserInfoUrl;

                var accessTokenInfo = await stateProvider.GetAccessTokenInfoAsync();

                if (accessTokenInfo is null)
                {
                    throw new Exception("Could not retrieve access token to get weather forecast.");
                }

                var token = accessTokenInfo.LoginResponse.AccessToken;
                var scheme = accessTokenInfo.LoginResponse.TokenType; //"Bearer"

                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(scheme))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
                    userInfo = await httpClient.GetFromJsonAsync<UserInfo>(userInfoUrl);
                }
                else
                {
                    Debug.WriteLine("Token or scheme is null or empty.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"HTTP Request error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
            UserInfo = userInfo;
        }

        public async Task UpdateProfilePictureAsync(IBrowserFile picture)
        {
            try
            {
                var httpClient = HttpClientHelper.GetHttpClient();
                var setPictureUrl = HttpClientHelper.UserPictureUpdateUrl;

                var accessTokenInfo = await stateProvider.GetAccessTokenInfoAsync();

                if (accessTokenInfo is null)
                {
                    throw new Exception("Could not retrieve access token to get weather forecast.");
                }

                var token = accessTokenInfo.LoginResponse.AccessToken;
                var scheme = accessTokenInfo.LoginResponse.TokenType; //"Bearer"
                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(scheme))
                {
                    var content = new MultipartFormDataContent();
                    content.Add(new StreamContent(picture.OpenReadStream()), "file", picture.Name);

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
                    var httpResponse = await httpClient.PostAsync(setPictureUrl, content);
                }
                else
                {
                    Debug.WriteLine("Token or scheme is null or empty.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"HTTP Request error: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public void Dispose() {        
            stateProvider.AuthenticationStateChanged += AuthenticationStateChanged;
        }
    }
}
