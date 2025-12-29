using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IStudentRepository: IBaseRepository<Student, int>
    {
        Task<Student> GetByEmailAsync(string email);
        Task<IEnumerable<Student>> GetByIdsAsync(List<int> ids);
        void DeleteRange(IEnumerable<Student> students);
        Task<Student?> GetStudentWithCoursesAsync(int studentId);

    }
}
