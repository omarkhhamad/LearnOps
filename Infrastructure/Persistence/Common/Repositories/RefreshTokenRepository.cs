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
            // Since RefreshToken is [Owned], it doesn't have its own DbSet.
            // We must query via the aggregate root (ApplicationUser).
            return await _context.Users
                .SelectMany(u => u.RefreshTokens)
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return new List<RefreshToken>();

            return user.RefreshTokens
                .Where(rt => rt.IsActive)
                .ToList();
        }

        public async Task RevokeAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            // When using Owned Types, just modifying the entity is enough if the tracker is aware.
            // However, to be safe with BaseRepository patterns:
            // _context.Entry(token).State = EntityState.Modified; // This might fail for owned types depending on EF version/tracking

            // Best practice for owned: Save via the owner. 
            // Assuming the token is already attached (which it should be if loaded via GetByTokenAsync)
            // nothing special needed other than SaveChanges which calls Commit.
            await Task.CompletedTask;
        }
    }
}
