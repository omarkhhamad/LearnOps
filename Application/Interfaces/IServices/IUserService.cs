using Application.Bases;
using Domain.Models;
using Application.DTOs.Authentication;

namespace Application.Interfaces.IServices
{
    public interface IUserService
    {
        Task<Result<UserDto?>> GetByIdAsync(Guid id);
        Task<Result<UserDto?>> GetByEmailAsync(string email);
        Task<Application.Bases.Result> CreateUserAsync(ApplicationUser user, string password, List<string> roles);
        Task<Application.Bases.Result> UpdateUserAsync(Guid id, UserDto dto);
        Task<Application.Bases.Result> DeleteUserAsync(Guid id);
        Task<Application.Bases.Result> DeleteUsersAsync(List<Guid> ids);
        Task<Result<bool>> UsernameExistsAsync(string username);
        Task<Result<bool>> EmailExistsAsync(string email);
        Task<Result<IList<string>>> GetRolesAsync(Guid userId);
    }
}
