using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Application.DTOs.Authentication.Requests;
using Application.DTOs.Authentication.Responses;
using Application.Interfaces.IServices;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Application.Options;
using Application.Bases;
using IAuthenticationService = Application.Interfaces.IServices.IAuthenticationService;

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
        private readonly GoogleAuthConfig _googleConfig;

        public AuthenticationController(
            IAuthenticationService authenticationService,
            IOptions<GoogleAuthConfig> googleConfig)
        {
            _authenticationService = authenticationService;
            _googleConfig = googleConfig.Value;
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
            var result = await _authenticationService.GoogleLoginAsync(request);
            return ToActionResult(result);
        }
    }
}
