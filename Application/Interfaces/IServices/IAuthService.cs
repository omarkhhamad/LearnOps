using Application.DTOs.Auth;
using Application.Result;

namespace Application.Interfaces.IServices
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
        Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenDto request);
        Task<Application.Result.Result> LogoutAsync(Guid userId);
        Task<Result<AuthResponse>> RegisterAsync(RegisterDto request);
    }
}
