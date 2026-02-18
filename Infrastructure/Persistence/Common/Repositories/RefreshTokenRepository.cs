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
    public class RefreshTokenRepository : BaseRepository<RefreshToken, Guid>, IRefreshTokenRepository
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
            // Avoid using the unmapped computed property `IsActive` inside the EF query
            // because it cannot be translated to SQL. Capture current UTC time and
            // evaluate the equivalent conditions directly so EF can translate them.
            var now = DateTime.UtcNow;
            return await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.Expiration >= now)
                .ToListAsync();
        }

        public async Task RevokeAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            // Mark modified so it will be updated on Commit
            _context.Entry(token).State = EntityState.Modified;
            await Task.CompletedTask;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
        }

        public async Task<List<RefreshToken>> GetByUserIdAsync(Guid userId)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();
        }
    }
}
