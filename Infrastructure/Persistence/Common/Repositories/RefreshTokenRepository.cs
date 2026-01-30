using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Common.Repositories
{
    public class RefreshTokenRepository :BaseRepository<RefreshToken,Guid> , IRefreshTokenRepository
    {

        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
        {
            return await _context.RefreshTokens
                .Where(rt =>
                    rt.UserId == userId &&
                    !rt.IsRevoked &&
                    rt.Expiration > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task RevokeAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            _context.RefreshTokens.Update(token);
            await Task.CompletedTask;
        }
    }
}
