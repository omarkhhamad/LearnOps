using Application.DTOs.Authentication.Requests;
using Application.DTOs.Authentication.Responses;
using Application.Bases;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Service interface for authentication operations
    /// Handles user registration, login, logout, and token refresh
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates a user with email and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Result containing authentication tokens if successful</returns>
        Task<Result<AuthenticationTokens>> LoginAsync(LoginRequest request);

        /// <summary>
        /// Registers a new user account
        /// </summary>
        /// <param name="request">Registration information</param>
        /// <returns>Result containing authentication tokens if successful</returns>
        Task<Result<AuthenticationTokens>> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Refreshes an expired access token using a valid refresh token
        /// </summary>
        /// <param name="request">Request containing the expired access token</param>
        /// <param name="refreshToken">The refresh token from HttpOnly cookie</param>
        /// <returns>Result containing new authentication tokens if successful</returns>
        Task<Result<AuthenticationTokens>> RefreshTokenAsync(RefreshTokenRequest request, string refreshToken);

        /// <summary>
        /// Logs out a user by revoking all active refresh tokens
        /// </summary>
        /// <param name="userId">User ID to logout</param>
        /// <returns>Result indicating success or failure</returns>
        Task<Result> LogoutAsync(Guid userId);
    }
}
