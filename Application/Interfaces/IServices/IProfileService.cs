using Application.Bases;
using Application.DTOs.Profile;
using System;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface IProfileService
    {
        Task<Result<UserProfileDto>> GetProfileAsync(Guid userId);
        Task<Result> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    }
}
