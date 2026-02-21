using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IRefreshTokenRepository : IBaseRepository<RefreshToken, Guid>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
        Task RevokeAsync(RefreshToken token);

        Task<List<RefreshToken>> GetByUserIdAsync(Guid userId);

    }

}
