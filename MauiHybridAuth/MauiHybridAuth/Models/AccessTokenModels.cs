using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MauiHybridAuth.Models
{
    /// <summary>
    /// This class represents the information related to an access token.
    /// </summary>
    public class AccessTokenInfo
    {
        public required string Email { get; set; }
        public required LoginToken LoginToken { get; set; }
        public required DateTime TokenExpiration { get; set; }
    }

    /// <summary>
    /// This class is used to store the token information received from the server.
    /// </summary>
    public class LoginToken
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
