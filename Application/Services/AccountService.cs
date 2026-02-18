using Application.Bases;
using Application.DTOs.Authentication.Requests;
using Application.DTOs.Profile;
using Application.Interfaces.IServices;
using Application.UnitOfWork;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AccountStatusDto>> GetAccountStatusAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return Result<AccountStatusDto>.Fail("User not found", 404);
                }

                var status = new AccountStatusDto
                {
                    Email = user.Email ?? string.Empty,
                    IsEmailConfirmed = user.EmailConfirmed,
                    HasPassword = await _userManager.HasPasswordAsync(user),
                    LastLoginAt = user.LastLoginAt,
                    TwoFactorEnabled = user.TwoFactorEnabled
                };

                return Result<AccountStatusDto>.Success(status);
            }
            catch (Exception ex)
            {
                return Result<AccountStatusDto>.Fail($"Error retrieving account status: {ex.Message}", 500);
            }
        }

        public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return Result.Fail("User not found", 404);
                }

                var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result.Fail(errors, 400);
                }

                return Result.Success(200, "Password changed successfully");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error changing password: {ex.Message}", 500);
            }
        }

        public async Task<Result> DeleteAccountAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return Result.Fail("User not found", 404);
                }

                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Result.Fail(string.Join(", ", result.Errors.Select(e => e.Description)), 400);
                }

                // Revoke all tokens
                var userWithTokens = await _unitOfWork.Users.GetByIdWithRefreshTokensAsync(userId);
                if (userWithTokens != null)
                {
                    foreach (var token in userWithTokens.RefreshTokens.Where(t => t.IsActive))
                    {
                        token.IsRevoked = true;
                    }
                    await _unitOfWork.CommitAsync();
                }

                return Result.Success(200, "Account deleted successfully");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error deleting account: {ex.Message}", 500);
            }
        }
    }
}
