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
        Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request);
        Task<Result<AuthenticationResponse>> RegisterAsync(RegisterRequest request);
        Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<Result> LogoutAsync(Guid userId);
        Task<Result<AuthenticationResponse>> GoogleLoginAsync(GoogleLoginRequest request);
    }
}
