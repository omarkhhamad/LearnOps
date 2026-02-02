using Application.Result;
using Domain.Models;
using Application.DTOs.Auth;

namespace Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<Result<IEnumerable<UserDto>>> GetAllAsync();
        Task<Result<UserDto?>> GetByIdAsync(Guid id);
        Task<Result<UserDto?>> GetByUsernameAsync(string username);
        Task<Result<UserDto?>> GetByEmailAsync(string email);
        Task<Application.Result.Result> CreateUserAsync(ApplicationUser user, string password, List<string> roles);
        Task<Application.Result.Result> UpdateUserAsync(Guid id, UserDto dto);
        Task<Application.Result.Result> DeleteUserAsync(Guid id);
        Task<Application.Result.Result> DeleteUsersAsync(List<Guid> ids);
        Task<Result<bool>> UsernameExistsAsync(string username);
        Task<Result<bool>> EmailExistsAsync(string email);

        Task<Application.Result.Result> UpdateUserRolesAsync(Guid userId, List<string> roles);
        Task<Result<IList<string>>> GetRolesAsync(Guid userId);
    }
}
