using System.Collections.Generic;
using System.Security.Claims;
using Domain.Models;

namespace Application.Interfaces.IServices
{
    /// <summary>
    /// Service interface for JWT token operations
    /// Handles generation and validation of access tokens and refresh tokens
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a JWT access token for the authenticated user
        /// </summary>
        /// <param name="user">The application user</param>
        /// <param name="roles">User's roles to include in token claims</param>
        /// <returns>JWT access token string</returns>
        string GenerateAccessToken(ApplicationUser user, IList<string> roles);

        /// <summary>
        /// Generates a new refresh token for the user
        /// </summary>
        /// <param name="userId">User ID to associate with the refresh token</param>
        /// <returns>RefreshToken entity</returns>
        RefreshToken GenerateRefreshToken(Guid userId);

        /// <summary>
        /// Validates and extracts claims from an expired access token
        /// Used during token refresh to verify the user's identity
        /// </summary>
        /// <param name="token">The expired access token</param>
        /// <returns>ClaimsPrincipal if valid, null otherwise</returns>
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
