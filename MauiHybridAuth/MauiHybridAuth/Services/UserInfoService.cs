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
    public class UserInfoService : IUserInfoService
    {
        private readonly ICustomAuthenticationStateProvider _authenticationStateProvider;

        public UserInfoService(ICustomAuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<UserInfo> GetUserInfoAsync()
        {
            UserInfo userInfo = new UserInfo();
            try
            {
                var httpClient = HttpClientHelper.GetHttpClient();
                var userInfoUrl = HttpClientHelper.UserInfoUrl;

                var loginToken = _authenticationStateProvider.AccessTokenInfo?.LoginToken;
                var token = loginToken?.AccessToken;
                var scheme = loginToken?.TokenType; //"Bearer"

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
            return userInfo;
        }
        public async Task UpdateProfilePictureAsync(IBrowserFile picture)
        {
            try
            {
                var httpClient = HttpClientHelper.GetHttpClient();
                var setPictureUrl = HttpClientHelper.UserPictureUpdateUrl;

                var loginToken = _authenticationStateProvider.AccessTokenInfo?.LoginToken;
                var token = loginToken?.AccessToken;
                var scheme = loginToken?.TokenType; //"Bearer"

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
    }
}
