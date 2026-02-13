using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IStudentRepository : IBaseRepository<Student, int>
    {
        Task<Student?> GetByEmailAsync(string email);
        Task<IEnumerable<Student>> GetByIdsAsync(List<int> ids);

        Task<Student?> GetByUserIdAsync(Guid userId);
        Task<bool> ExistsByUserIdAsync(Guid userId);
        Task<Student?> GetStudentWithCoursesAsync(int studentId);

    }
}
