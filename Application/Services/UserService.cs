using Application.Interfaces.IServices;
using Application.UnitOfWork;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Application.Result;
using Application.DTOs.Auth;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        private async Task<UserDto> MapToDtoAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList()
            };
        }

        // =================== CRUD ===================
        public async Task<Result<IEnumerable<UserDto>>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var dtos = new List<UserDto>();
            foreach (var user in users)
            {
                dtos.Add(await MapToDtoAsync(user));
            }
            return Result<IEnumerable<UserDto>>.Success(dtos);
        }

        public async Task<Result<UserDto?>> GetByIdAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return Result<UserDto?>.Fail("User not found", 404);
            return Result<UserDto?>.Success(await MapToDtoAsync(user));
        }

        public async Task<Result<UserDto?>> GetByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return Result<UserDto?>.Fail("User not found", 404);
            return Result<UserDto?>.Success(await MapToDtoAsync(user));
        }

        public async Task<Result<UserDto?>> GetByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Result<UserDto?>.Fail("User not found", 404);
            return Result<UserDto?>.Success(await MapToDtoAsync(user));
        }

        public async Task<Application.Result.Result> CreateUserAsync(ApplicationUser user, string password, List<string> roles)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Application.Result.Result.Fail(string.Join(", ", result.Errors.Select(e => e.Description)), 400);
                }

                if (roles != null && roles.Any())
                {
                    var roleResult = await _userManager.AddToRolesAsync(user, roles);
                    if (!roleResult.Succeeded)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return Application.Result.Result.Fail(string.Join(", ", roleResult.Errors.Select(e => e.Description)), 400);
                    }

                    await SyncProfilesAsync(user.Id, roles);
                }

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();
                return Application.Result.Result.Success(201, "User created successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Application.Result.Result.Fail(ex.Message, 500);
            }
        }

        public async Task<Application.Result.Result> UpdateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Application.Result.Result.Fail(string.Join(", ", result.Errors.Select(e => e.Description)), 400);

            await _unitOfWork.CommitAsync();
            return Application.Result.Result.Success(200, "User updated successfully");
        }

        public async Task<Application.Result.Result> UpdateUserAsync(Guid id, UserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return Application.Result.Result.Fail("User not found", 404);

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UserName = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Application.Result.Result.Fail(string.Join(", ", result.Errors.Select(e => e.Description)), 400);

            await _unitOfWork.CommitAsync();
            return Application.Result.Result.Success(200, "User updated successfully");
        }

        public async Task<Application.Result.Result> DeleteUserAsync(Guid id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null) return Application.Result.Result.Fail("User not found", 404);

                var student = await _unitOfWork.Students.GetByUserIdAsync(id);
                if (student != null) _unitOfWork.Students.Delete(student);

                var instructor = await _unitOfWork.Instructors.GetByUserIdAsync(id);
                if (instructor != null) _unitOfWork.Instructors.Delete(instructor);

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return Application.Result.Result.Fail(string.Join(", ", result.Errors.Select(e => e.Description)), 400);
                }

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();
                return Application.Result.Result.Success(200, "User deleted successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Application.Result.Result.Fail(ex.Message, 500);
            }
        }

        public async Task<Application.Result.Result> DeleteUsersAsync(List<Guid> ids)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var id in ids)
                {
                    var user = await _userManager.FindByIdAsync(id.ToString());
                    if (user == null) continue;

                    var student = await _unitOfWork.Students.GetByUserIdAsync(id);
                    if (student != null) _unitOfWork.Students.Delete(student);

                    var instructor = await _unitOfWork.Instructors.GetByUserIdAsync(id);
                    if (instructor != null) _unitOfWork.Instructors.Delete(instructor);

                    var result = await _userManager.DeleteAsync(user);
                    if (!result.Succeeded)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return Application.Result.Result.Fail($"Failed to delete user {id}: " + string.Join(", ", result.Errors.Select(e => e.Description)), 400);
                    }
                }

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();
                return Application.Result.Result.Success(200, "Users deleted successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Application.Result.Result.Fail(ex.Message, 500);
            }
        }

        // =================== Check existence ===================
        public async Task<Result<bool>> UsernameExistsAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            return Result<bool>.Success(user != null);
        }

        public async Task<Result<bool>> EmailExistsAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return Result<bool>.Success(user != null);
        }

        // =================== Roles Management ===================
        public async Task<Result<IList<string>>> GetRolesAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return Result<IList<string>>.Fail("User not found", 404);
            var roles = await _userManager.GetRolesAsync(user);
            return Result<IList<string>>.Success(roles);
        }

        public async Task<Application.Result.Result> UpdateUserRolesAsync(Guid userId, List<string> roles)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null) return Application.Result.Result.Fail("User not found", 404);

                var currentRoles = await _userManager.GetRolesAsync(user);

                var rolesToAdd = roles.Except(currentRoles).ToList();
                var rolesToRemove = currentRoles.Except(roles).ToList();

                if (rolesToAdd.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!addResult.Succeeded)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return Application.Result.Result.Fail(string.Join(", ", addResult.Errors.Select(e => e.Description)), 400);
                    }
                }

                if (rolesToRemove.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!removeResult.Succeeded)
                    {
                        await _unitOfWork.RollbackTransactionAsync();
                        return Application.Result.Result.Fail(string.Join(", ", removeResult.Errors.Select(e => e.Description)), 400);
                    }
                }

                await SyncProfilesAsync(userId, roles, currentRoles.ToList());

                await _unitOfWork.CommitAsync();
                await _unitOfWork.CommitTransactionAsync();
                return Application.Result.Result.Success(200, "User roles updated successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Application.Result.Result.Fail(ex.Message, 500);
            }
        }

        private async Task SyncProfilesAsync(Guid userId, List<string> newRoles, List<string>? currentRoles = null)
        {
            currentRoles ??= new List<string>();

            var addedRoles = newRoles.Except(currentRoles).ToList();
            var removedRoles = currentRoles.Except(newRoles).ToList();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return;

            foreach (var role in addedRoles)
            {
                if (role.Equals("Student", StringComparison.OrdinalIgnoreCase) && !await _unitOfWork.Students.ExistsByUserIdAsync(userId))
                {
                    await _unitOfWork.Students.AddAsync(new Student
                    {
                        UserId = userId,
                        FullName = user.FullName ?? user.UserName ?? "Unknown",
                        Email = user.Email ?? "no-email@example.com",
                        Phone = user.PhoneNumber ?? "0000000000",
                        CreatedAt = DateTime.UtcNow
                    });
                }
                else if (role.Equals("Instructor", StringComparison.OrdinalIgnoreCase) && !await _unitOfWork.Instructors.ExistsByUserIdAsync(userId))
                {
                    await _unitOfWork.Instructors.AddAsync(new Instructor
                    {
                        UserId = userId,
                        FullName = user.FullName ?? user.UserName ?? "Unknown",
                        Email = user.Email ?? "no-email@example.com",
                        Phone = user.PhoneNumber ?? "0000000000"
                    });
                }
            }

            foreach (var role in removedRoles)
            {
                if (role.Equals("Student", StringComparison.OrdinalIgnoreCase))
                {
                    var student = await _unitOfWork.Students.GetByUserIdAsync(userId);
                    if (student != null) _unitOfWork.Students.Delete(student);
                }
                else if (role.Equals("Instructor", StringComparison.OrdinalIgnoreCase))
                {
                    var instructor = await _unitOfWork.Instructors.GetByUserIdAsync(userId);
                    if (instructor != null) _unitOfWork.Instructors.Delete(instructor);
                }
            }
        }
    }
}
