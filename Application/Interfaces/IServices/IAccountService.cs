using Application.Bases;
using Application.DTOs.Authentication.Requests;
using Application.DTOs.Profile;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface IAccountService
    {
        Task<Result<AccountStatusDto>> GetAccountStatusAsync(Guid userId);
        Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
        Task<Result> DeleteAccountAsync(Guid userId);
    }
}
