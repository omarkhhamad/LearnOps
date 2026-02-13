namespace Application.DTOs.Authentication.Responses
{
    /// <summary>
    /// Public API response for authentication operations (Login, Register, Refresh)
    /// This is returned to the client - does NOT include refresh token (stored in HttpOnly cookie)
    /// </summary>
    public class AuthenticationResponse
    {
        /// <summary>
        /// JWT access token for API authentication
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Access token expiration timestamp (UTC)
        /// </summary>
        public DateTime AccessTokenExpiration { get; set; }
    }
}
