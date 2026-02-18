using Application.Bases;
using Application.DTOs.Authentication.Requests;
using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Get current user's account security status
        /// </summary>
        [HttpGet("status")]
        public async Task<IActionResult> GetAccountStatus()
        {
            var userId = GetUserId();
            var result = await _accountService.GetAccountStatusAsync(userId);
            return ToActionResult(result);
        }

        /// <summary>
        /// Change the current user's password
        /// </summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var userId = GetUserId();
            var result = await _accountService.ChangePasswordAsync(userId, request);
            return ToActionResult(result);
        }

        /// <summary>
        /// Soft delete the current user's account
        /// </summary>
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = GetUserId();
            var result = await _accountService.DeleteAccountAsync(userId);
            return ToActionResult(result);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");
            if (userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return Guid.Parse(userIdClaim.Value);
        }
    }
}
