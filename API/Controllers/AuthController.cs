using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Auth;
using Application.Interfaces.IServices;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return ToActionResult(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return ToActionResult(result);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return ToActionResult(result);
        }

        [HttpPost("logout/{userId:guid}")]
        public async Task<IActionResult> Logout(Guid userId)
        {
            var result = await _authService.LogoutAsync(userId);
            return ToActionResult(result);
        }
    }
}
