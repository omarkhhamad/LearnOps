using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Common.Repositories
{
    public class BaseRepository<T, TKey> : IBaseRepository<T, TKey> where T : class
    {
        protected readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _context.Set<T>().ToListAsync();

        public async Task<T?> GetByIdAsync(TKey id)
            => await _context.Set<T>().FindAsync(id);

        public async Task AddAsync(T entity)
            => await _context.Set<T>().AddAsync(entity);

        public async Task AddRangeAsync(ICollection<T> entities)
            => await _context.Set<T>().AddRangeAsync(entities);

        public void Update(T entity)
            => _context.Set<T>().Update(entity);

        public void Delete(T entity)
            => _context.Set<T>().Remove(entity);
    }
}
