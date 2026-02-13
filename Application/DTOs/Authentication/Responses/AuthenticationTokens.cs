namespace Application.DTOs.Authentication.Responses
{
    /// <summary>
    /// Internal DTO used for passing tokens from Service layer to Controller layer
    /// Contains both access token and refresh token
    /// The controller will extract the refresh token and store it in HttpOnly cookie
    /// </summary>
    public class AuthenticationTokens
    {
        /// <summary>
        /// JWT access token for API authentication
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Refresh token for obtaining new access tokens
        /// This should be stored in HttpOnly cookie, not returned in response body
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Access token expiration timestamp (UTC)
        /// </summary>
        public DateTime AccessTokenExpiration { get; set; }
    }
}
