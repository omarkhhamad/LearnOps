using System;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Auth;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Options;
using Application.UnitOfWork;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Application.Result;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserService _userService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> options,
            IUserService userService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
            _jwtSettings = options.Value;
            _userService = userService;
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                    return Result<AuthResponse>.Fail("Invalid credentials", 401);

                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = _jwtService.GenerateAccessToken(user, roles);
                var refreshToken = _jwtService.GenerateRefreshToken(user.Id);

                await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
                await _unitOfWork.CommitAsync();

                return Result<AuthResponse>.Success(new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
                });
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.Fail(ex.Message, 500);
            }
        }

        public async Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenDto request)
        {
            try
            {
                var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
                if (principal == null) return Result<AuthResponse>.Fail("Invalid access token", 400);

                var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null) return Result<AuthResponse>.Fail("Invalid token claims", 400);

                var userId = Guid.Parse(userIdClaim.Value);
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null) return Result<AuthResponse>.Fail("User not found", 404);

                var existingToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);
                if (existingToken == null || existingToken.IsRevoked || existingToken.Expiration < DateTime.UtcNow)
                    return Result<AuthResponse>.Fail("Invalid or expired refresh token", 400);

                existingToken.IsRevoked = true;

                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = _jwtService.GenerateAccessToken(user, roles);
                var newRefreshToken = _jwtService.GenerateRefreshToken(user.Id);

                await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
                await _unitOfWork.CommitAsync();

                return Result<AuthResponse>.Success(new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = newRefreshToken.Token,
                    AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
                });
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.Fail(ex.Message, 500);
            }
        }

        public async Task<Application.Result.Result> LogoutAsync(Guid userId)
        {
            try
            {
                var tokens = await _unitOfWork.RefreshTokens.GetActiveByUserIdAsync(userId);
                foreach (var token in tokens)
                    token.IsRevoked = true;

                await _unitOfWork.CommitAsync();
                return Application.Result.Result.Success(200, "Logged out successfully");
            }
            catch (Exception ex)
            {
                return Application.Result.Result.Fail(ex.Message, 500);
            }
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterDto request)
        {
            try
            {
                if (request.Password != request.ConfirmPassword)
                    return Result<AuthResponse>.Fail("Passwords do not match", 400);

                var user = new ApplicationUser
                {
                    Email = request.Email,
                    UserName = request.UserName,
                    FullName = request.FullName,
                    CreatedAt = DateTime.UtcNow
                };

                var roles = new List<string> { "Student" };
                var createResult = await _userService.CreateUserAsync(user, request.Password, roles);
                if (!createResult.IsSuccess)
                    return Result<AuthResponse>.Fail(createResult.Message, createResult.StatusCode ?? 400);

                var registeredRoles = await _userManager.GetRolesAsync(user);
                var accessToken = _jwtService.GenerateAccessToken(user, registeredRoles);
                var refreshToken = _jwtService.GenerateRefreshToken(user.Id);

                await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
                await _unitOfWork.CommitAsync();

                return Result<AuthResponse>.Success(new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
                }, 201);
            }
            catch (Exception ex)
            {
                return Result<AuthResponse>.Fail(ex.Message, 500);
            }
        }
    }
}
