using Application.Bases;
using Application.DTOs.Profile;
using Application.Interfaces.IServices;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : BaseController
    {
        private readonly IProfileService _profileService;
        private readonly IFileService _fileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(IProfileService profileService, IFileService fileService, UserManager<ApplicationUser> userManager)
        {
            _profileService = profileService;
            _fileService = fileService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get the current user's profile
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetUserId();
            var result = await _profileService.GetProfileAsync(userId);
            return ToActionResult(result);
        }

        /// <summary>
        /// Update the current user's profile
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userId = GetUserId();
            var result = await _profileService.UpdateProfileAsync(userId, request);
            return ToActionResult(result);
        }

        /// <summary>
        /// Upload a profile picture
        /// </summary>
        [HttpPost("upload-picture")]
        public async Task<IActionResult> UploadPicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var userId = GetUserId();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound("User not found");

            // Optional: delete old picture if it was a local file
            if (!string.IsNullOrEmpty(user.ProfilePictureUrl) && !user.ProfilePictureUrl.StartsWith("http"))
            {
                _fileService.DeleteFile(user.ProfilePictureUrl, "uploads/profiles");
            }

            var path = await _fileService.SaveFileAsync(file, "uploads/profiles");
            user.ProfilePictureUrl = path;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(string.Join(", ", result.Errors.Select(e => e.Description)));

            return Ok(new { url = path });
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
