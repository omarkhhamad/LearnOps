using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
namespace Application.Interfaces.IRepositories
{
    public interface ICourseRepository: IBaseRepository<Course, int>
    {
        Task<IEnumerable<Course>> GetByIdsAsync(List<int> ids);
        void DeleteRange(IEnumerable<Course> courses);
    }
}
