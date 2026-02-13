using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IInstructorRepository : IBaseRepository<Instructor, int>
    {
        Task<IEnumerable<Instructor>> GetByIdsAsync(List<int> ids);

        Task<Instructor?> GetByUserIdAsync(Guid userId);
        Task<bool> ExistsByUserIdAsync(Guid userId);
        Task<Instructor?> GetInstructorWithCoursesAndGroupsAsync(int id);
    }
}
