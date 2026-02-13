using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace Infrastructure.Persistence.Common.Repositories
{
    public class BaseRepository<T, TKey> : IBaseRepository<T, TKey> where T : class
    {
        protected readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _context.Set<T>().ToListAsync();

        public virtual async Task<T?> GetByIdAsync(TKey id)
        {
            var keyName = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties.Select(x => x.Name).Single();
            if (keyName == null) return await _context.Set<T>().FindAsync(id);

            return await _context.Set<T>().FirstOrDefaultAsync(e => object.Equals(EF.Property<TKey>(e, keyName), id));
        }

        public async Task AddAsync(T entity)
            => await _context.Set<T>().AddAsync(entity);

        public async Task AddRangeAsync(ICollection<T> entities)
            => await _context.Set<T>().AddRangeAsync(entities);

        public void Update(T entity)
            => _context.Set<T>().Update(entity);

        public virtual void Delete(T entity)
        {
            if (entity is ISoftDeletable softDeletable)
            {
                softDeletable.IsDeleted = true;
                softDeletable.DeletedAt = DateTime.UtcNow;
                _context.Set<T>().Update(entity);
            }
            else
            {
                _context.Set<T>().Remove(entity);
            }
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }
    }
}
