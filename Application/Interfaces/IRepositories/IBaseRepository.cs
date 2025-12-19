using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IRepositories
{
    public interface IBaseRepository<T, TKey> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(TKey id);
        Task AddAsync(T entity);
        Task AddRangeAsync(ICollection<T> entities);
        void Update(T entity);
        void Delete(T entity);
    }
}
