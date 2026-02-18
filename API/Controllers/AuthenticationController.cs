using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Authentication.Requests;
using Application.DTOs.Authentication.Responses;
using Application.Interfaces.IServices;
using System;
using System.Threading.Tasks;
using Application.Options;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace API.Controllers
{
    /// <summary>
    /// Controller for authentication operations
    /// Handles user registration, login, token refresh, and logout
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly JwtSettings _jwtSettings;

        public AuthenticationController(IAuthenticationService authenticationService, IOptions<JwtSettings> jwtOptions)
        {
            _authenticationService = authenticationService;
            _jwtSettings = jwtOptions.Value;
        }

        /// <summary>
        /// Register a new user account
        /// </summary>
        /// <param name="request">Registration details</param>
        /// <returns>Authentication response with access token</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authenticationService.RegisterAsync(request);

            if (result.IsSuccess && result.Data != null)
            {
                // Store refresh token in HttpOnly cookie (secure)
                SetRefreshTokenCookie(result.Data.RefreshToken);

                // Return only the public response (without refresh token)
                var publicResponse = new AuthenticationResponse
                {
                    AccessToken = result.Data.AccessToken,
                    AccessTokenExpiration = result.Data.AccessTokenExpiration
                };

                return ToActionResult(Application.Bases.Result<AuthenticationResponse>.Success(publicResponse, result.StatusCode));
            }

            return ToActionResult(Application.Bases.Result<AuthenticationResponse>.Fail(result.Message, result.StatusCode));
        }

        /// <summary>
        /// Login with email and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>Authentication response with access token</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authenticationService.LoginAsync(request);

            if (result.IsSuccess && result.Data != null)
            {
                // Store refresh token in HttpOnly cookie (secure)
                SetRefreshTokenCookie(result.Data.RefreshToken);

                // Return only the public response (without refresh token)
                var publicResponse = new AuthenticationResponse
                {
                    AccessToken = result.Data.AccessToken,
                    AccessTokenExpiration = result.Data.AccessTokenExpiration
                };

                return ToActionResult(Application.Bases.Result<AuthenticationResponse>.Success(publicResponse, result.StatusCode));
            }

            return ToActionResult(Application.Bases.Result<AuthenticationResponse>.Fail(result.Message, result.StatusCode));
        }

        /// <summary>
        /// Refresh access token using refresh token from cookie
        /// </summary>
        /// <param name="request">Request containing expired access token</param>
        /// <returns>New authentication response with fresh access token</returns>
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            // Get refresh token from HttpOnly cookie
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return ToActionResult(Application.Bases.Result<AuthenticationResponse>.Fail("Refresh token not found", 401));
            }

            var result = await _authenticationService.RefreshTokenAsync(request, refreshToken);

            if (result.IsSuccess && result.Data != null)
            {
                // Store new refresh token in HttpOnly cookie
                SetRefreshTokenCookie(result.Data.RefreshToken);

                // Return only the public response (without refresh token)
                var publicResponse = new AuthenticationResponse
                {
                    AccessToken = result.Data.AccessToken,
                    AccessTokenExpiration = result.Data.AccessTokenExpiration
                };

                return ToActionResult(Application.Bases.Result<AuthenticationResponse>.Success(publicResponse, result.StatusCode));
            }

            return ToActionResult(Application.Bases.Result<AuthenticationResponse>.Fail(result.Message, result.StatusCode));
        }

        /// <summary>
        /// Logout user by revoking all refresh tokens
        /// </summary>
        /// <param name="userId">User ID to logout</param>
        /// <returns>Success or failure result</returns>
        [HttpPost("logout/{userId:guid}")]
        public async Task<IActionResult> Logout(Guid userId)
        {
            var result = await _authenticationService.LogoutAsync(userId);

            if (result.IsSuccess)
            {
                // Clear refresh token cookie
                Response.Cookies.Delete("refreshToken");
            }

            return ToActionResult(result);
        }

        /// <summary>
        /// Sets refresh token in HttpOnly cookie for security
        /// </summary>
        /// <param name="refreshToken">Refresh token to store</param>
        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,                                // Cannot be accessed by JavaScript (XSS protection)
                Secure = true,                                  // Only sent over HTTPS
                SameSite = SameSiteMode.Strict,                 // CSRF protection
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays) // Match refresh token expiration from settings
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
