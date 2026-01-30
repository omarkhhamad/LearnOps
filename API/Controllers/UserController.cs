using Application.DTOs.Auth;
using Application.Interfaces.IServices;
using Application.Result;
using Result = Application.Result.Result;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return ToActionResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);
            return ToActionResult(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email, // Use Email as UserName
                PhoneNumber = dto.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userService.CreateUserAsync(user, dto.Password, dto.Roles);
            return ToActionResult(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDto dto)
        {
            var result = await _userService.UpdateUserAsync(id, dto);
            return ToActionResult(result);
        }

        [HttpPut("{id}/roles")]
        public async Task<IActionResult> UpdateRoles(Guid id, [FromBody] UpdateUserRolesDto dto)
        {
            var result = await _userService.UpdateUserRolesAsync(id, dto.Roles);
            return ToActionResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return ToActionResult(result);
        }

        [HttpDelete("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] List<Guid> ids)
        {
            var result = await _userService.DeleteUsersAsync(ids);
            return ToActionResult(result);
        }
    }
}
