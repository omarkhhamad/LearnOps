using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authenticationService.RegisterAsync(request);
            return ToActionResult(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authenticationService.LoginAsync(request);
            return ToActionResult(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var result = await _authenticationService.RefreshTokenAsync(request);
            return ToActionResult(result);
        }

        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                             ?? User.FindFirst("sub");

            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var result = await _authenticationService.LogoutAsync(Guid.Parse(userIdClaim.Value));
            return ToActionResult(result);
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
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
