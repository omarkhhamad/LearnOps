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
using System.Net.Http;
using System.Net.Http.Json;

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
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwtOptions,
            IOptions<GoogleAuthConfig> googleOptions,
            IUserService userService,
            IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtOptions.Value;
            _googleConfig = googleOptions.Value;
            _userService = userService;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Authenticates user and generates tokens
        /// </summary>
        public async Task<Result<AuthenticationResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    return Result<AuthenticationResponse>.Fail("Invalid email or password", 401);
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
                var user = await _unitOfWork.Users.GetByIdWithRefreshTokensAsync(userId);

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
                return Result<AuthenticationResponse>.Fail($"Token refresh failed: {ex.Message}", 500);
            }
        }

        public async Task<Result> LogoutAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdWithRefreshTokensAsync(userId);
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
                GoogleJsonWebSignature.Payload payload = null;

                try
                {
                    var settings = new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new List<string> { _googleConfig.ClientId }
                    };

                    payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
                }
                catch (InvalidJwtException)
                {
                    // Fallback: Try validating as Access Token
                    try
                    {
                        var client = _httpClientFactory.CreateClient();
                        var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v3/userinfo");
                        requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", request.IdToken);

                        var userInfoResponse = await client.SendAsync(requestMessage);

                        if (userInfoResponse.IsSuccessStatusCode)
                        {
                            var googleUser = await userInfoResponse.Content.ReadFromJsonAsync<GoogleUserInfo>();

                            if (googleUser != null)
                            {
                                payload = new GoogleJsonWebSignature.Payload
                                {
                                    Email = googleUser.email,
                                    Name = googleUser.name,
                                    Picture = googleUser.picture,
                                    EmailVerified = googleUser.email_verified
                                };
                            }
                        }
                    }
                    catch
                    {
                        // Ignore access token validation errors and let it fall through to "Invalid Google token"
                    }
                }

                if (payload == null)
                {
                    return Result<AuthenticationResponse>.Fail("Invalid Google token", 400);
                }

                if (!payload.EmailVerified)
                {
                    return Result<AuthenticationResponse>.Fail("Google email is not verified", 400);
                }

                var user = await _unitOfWork.Users.GetByEmailAsync(payload.Email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        Email = payload.Email,
                        UserName = payload.Email,
                        FullName = payload.Name ?? payload.Email,
                        ProfilePictureUrl = payload.Picture,
                        CreatedAt = DateTime.UtcNow
                    };

                    var roles = new List<string> { "Student" };
                    var createResult = await _userService.CreateUserAsync(user, Guid.NewGuid().ToString("N") + "A1!", roles);

                    if (!createResult.IsSuccess)
                    {
                        return Result<AuthenticationResponse>.Fail(createResult.Message, createResult.StatusCode ?? 400);
                    }
                }
                else if (string.IsNullOrEmpty(user.ProfilePictureUrl) && !string.IsNullOrEmpty(payload.Picture))
                {
                    user.ProfilePictureUrl = payload.Picture;
                    await _userManager.UpdateAsync(user);
                }

                return await GenerateAuthResponseAsync(user);
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

            // Update last login time
            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);


            // Directly add the refresh token to the repository to ensure it's treated as a new INSERT
            // and doesn't trigger concurrency checks on the ApplicationUser
            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.CommitAsync();

            return Result<AuthenticationResponse>.Success(new AuthenticationResponse
            {
                AccessToken = accessToken,
                AccessTokenExpiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
            }, successStatusCode);
        }
    }

    public class GoogleUserInfo
    {
        public string sub { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string picture { get; set; }
        public string email { get; set; }
        public bool email_verified { get; set; }
        public string locale { get; set; }
    }
}
