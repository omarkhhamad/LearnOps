using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Auth;
using Domain.Models;

namespace Application.Interfaces.IServices
{
    public interface IJwtService
    {
        string GenerateAccessToken(ApplicationUser user, IList<string> roles);
        RefreshToken GenerateRefreshToken(Guid userId);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
