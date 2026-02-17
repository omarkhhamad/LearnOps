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
using Google.Apis.Auth;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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
        private readonly GoogleAuthConfig _googleConfig;
        private readonly JwtSettings _jwtSettings;
        private readonly IUserService _userService;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwtOptions,
            IOptions<GoogleAuthConfig> googleOptions,
            IUserService userService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtOptions.Value;
            _googleConfig = googleOptions.Value;
            _userService = userService;
        }

        /// <summary>
        /// Authenticates user and generates tokens
        /// </summary>
        public async Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    return Result<AuthenticationResponse>.Fail("Invalid email or password", 401);
                }

                return await GenerateAuthResponseAsync(user);
            }
            catch (Exception ex)
            {
                return Result<AuthenticationResponse>.Fail($"Login failed: {ex.Message}", 500);
            }
        }

        public async Task<Result<AuthenticationResponse>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                if (request.Password != request.ConfirmPassword)
                {
                    return Result<AuthenticationResponse>.Fail("Passwords do not match", 400);
                }

                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return Result<AuthenticationResponse>.Fail("Email already exists", 400);
                }

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
                {
                    return Result<AuthenticationResponse>.Fail(createResult.Message, createResult.StatusCode ?? 400);
                }

                return await GenerateAuthResponseAsync(user, 201);
            }
            catch (Exception ex)
            {
                return Result<AuthenticationResponse>.Fail($"Registration failed: {ex.Message}", 500);
            }
        }

        public async Task<Result<AuthenticationResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
                if (principal == null)
                {
                    return Result<AuthenticationResponse>.Fail("Invalid access token", 400);
                }

                var jti = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                var userIdClaim = principal.Claims.FirstOrDefault(c =>
                    c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(jti) || userIdClaim == null)
                {
                    return Result<AuthenticationResponse>.Fail("Invalid token claims", 400);
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    return Result<AuthenticationResponse>.Fail("User not found", 404);
                }

                var existingToken = user.RefreshTokens.FirstOrDefault(t => t.Token == jti);

                if (existingToken == null || !existingToken.IsActive)
                {
                    return Result<AuthenticationResponse>.Fail("Invalid or expired session", 400);
                }

                existingToken.IsRevoked = true;
                return await GenerateAuthResponseAsync(user);
            }
            catch (Exception ex)
            {
                return Result<AuthenticationResponse>.Fail($"Token refresh failed: {ex.Message}", 500);
            }
        }

        public async Task<Result> LogoutAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return Result.Fail("User not found", 404);
                }

                var activeTokens = user.RefreshTokens.Where(t => t.IsActive).ToList();
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

        public async Task<Result<AuthenticationResponse>> GoogleLoginAsync(GoogleLoginRequest request)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new List<string> { _googleConfig.ClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

                if (payload == null)
                {
                    return Result<AuthenticationResponse>.Fail("Invalid Google token", 400);
                }

                if (!payload.EmailVerified)
                {
                    return Result<AuthenticationResponse>.Fail("Google email is not verified", 400);
                }

                var user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        FullName = payload.Name ?? payload.Email,
                        CreatedAt = DateTime.UtcNow
                    };

                    var roles = new List<string> { "Student" };
                    var createResult = await _userService.CreateUserAsync(user, Guid.NewGuid().ToString("N") + "A1!", roles);

                    if (!createResult.IsSuccess)
                    {
                        return Result<AuthenticationResponse>.Fail(createResult.Message, createResult.StatusCode ?? 400);
                    }
                }

                return await GenerateAuthResponseAsync(user);
            }
            catch (InvalidJwtException ex)
            {
                return Result<AuthenticationResponse>.Fail($"Invalid Google token: {ex.Message}", 400);
            }
            catch (Exception ex)
            {
                return Result<AuthenticationResponse>.Fail($"Google login failed: {ex.Message}", 500);
            }
        }

        private async Task<Result<AuthenticationResponse>> GenerateAuthResponseAsync(ApplicationUser user, int successStatusCode = 200)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(accessToken);
            var jti = jwtToken.Id;

            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);
            refreshToken.Token = jti ?? Guid.NewGuid().ToString();

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
            await _unitOfWork.CommitAsync();

            return Result<AuthenticationResponse>.Success(new AuthenticationResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
            }, successStatusCode);
        }
    }
}
