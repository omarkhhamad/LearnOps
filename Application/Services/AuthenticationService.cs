using System;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Authentication.Requests;
using Application.DTOs.Authentication.Responses;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;
using Application.Options;
using Application.UnitOfWork;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Application.Bases;

namespace Infrastructure.Services
{
    /// <summary>
    /// Service for authentication operations including login, registration, and token management
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserService _userService;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> options,
            IUserService userService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _jwtSettings = options.Value;
            _userService = userService;
        }

        /// <summary>
        /// Authenticates user and generates tokens
        /// </summary>
        public async Task<Result<AuthenticationTokens>> LoginAsync(LoginRequest request)
        {
            try
            {
                // Validate user credentials
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    return Result<AuthenticationTokens>.Fail("Invalid email or password", 401);
                }

                // Generate tokens
                var roles = await _userManager.GetRolesAsync(user);
                var accessToken = _tokenService.GenerateAccessToken(user, roles);
                var refreshToken = _tokenService.GenerateRefreshToken(user.Id);
                // Store refresh token as separate entity
                await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
                await _unitOfWork.CommitAsync();

                return Result<AuthenticationTokens>.Success(new AuthenticationTokens
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
                });
            }
            catch (Exception ex)
            {
                return Result<AuthenticationTokens>.Fail($"Login failed: {ex.Message}", 500);
            }
        }

        /// <summary>
        /// Registers a new user and generates tokens
        /// </summary>
        public async Task<Result<AuthenticationTokens>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Validate password confirmation
                if (request.Password != request.ConfirmPassword)
                {
                    return Result<AuthenticationTokens>.Fail("Passwords do not match", 400);
                }

                // Create user entity
                var user = new ApplicationUser
                {
                    Email = request.Email,
                    UserName = request.UserName,
                    FullName = request.FullName,
                    CreatedAt = DateTime.UtcNow
                };

                // Create user with default role
                var roles = new List<string> { "Student" };
                var createResult = await _userService.CreateUserAsync(user, request.Password, roles);

                if (!createResult.IsSuccess)
                {
                    return Result<AuthenticationTokens>.Fail(createResult.Message, createResult.StatusCode ?? 400);
                }

                // Generate tokens
                var registeredRoles = await _userManager.GetRolesAsync(user);
                var accessToken = _tokenService.GenerateAccessToken(user, registeredRoles);
                var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

                // Store refresh token as separate entity
                await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
                await _unitOfWork.CommitAsync();

                return Result<AuthenticationTokens>.Success(new AuthenticationTokens
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
                }, 201);
            }
            catch (Exception ex)
            {
                return Result<AuthenticationTokens>.Fail($"Registration failed: {ex.Message}", 500);
            }
        }

        /// <summary>
        /// Refreshes access token using a valid refresh token
        /// </summary>
        public async Task<Result<AuthenticationTokens>> RefreshTokenAsync(RefreshTokenRequest request, string refreshToken)
        {
            try
            {
                // Validate and extract claims from expired access token
                var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
                if (principal == null)
                {
                    return Result<AuthenticationTokens>.Fail("Invalid access token", 400);
                }

                // Extract user ID from claims
                var userIdClaim = principal.Claims.FirstOrDefault(c =>
                    c.Type == "sub" || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);

                if (userIdClaim == null)
                {
                    return Result<AuthenticationTokens>.Fail("Invalid token claims", 400);
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var user = await _unitOfWork.Users.GetByIdAsync(userId);

                if (user == null)
                {
                    return Result<AuthenticationTokens>.Fail("User not found", 404);
                }

                // Validate refresh token
                var existingToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);
                if (existingToken == null || !existingToken.IsActive)
                {
                    return Result<AuthenticationTokens>.Fail("Invalid or expired refresh token", 400);
                }

                // Revoke old refresh token
                existingToken.IsRevoked = true;
                await _unitOfWork.CommitAsync();

                // Generate new tokens
                var roles = await _userManager.GetRolesAsync(user);
                var newAccessToken = _tokenService.GenerateAccessToken(user, roles);
                var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

                // Store new refresh token as separate entity
                await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);
                await _unitOfWork.CommitAsync();

                return Result<AuthenticationTokens>.Success(new AuthenticationTokens
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken.Token,
                    AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
                });
            }
            catch (Exception ex)
            {
                return Result<AuthenticationTokens>.Fail($"Token refresh failed: {ex.Message}", 500);
            }
        }

        /// <summary>
        /// Logs out user by revoking all active refresh tokens
        /// </summary>
        public async Task<Result> LogoutAsync(Guid userId)
        {
            try
            {
                // Revoke all active refresh tokens for the user
                var activeTokens = await _unitOfWork.RefreshTokens.GetActiveByUserIdAsync(userId);

                foreach (var token in activeTokens)
                {
                    token.IsRevoked = true;
                }

                await _unitOfWork.CommitAsync();

                return Result.Success(200, "Logged out successfully");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Logout failed: {ex.Message}", 500);
            }
        }
    }
}
