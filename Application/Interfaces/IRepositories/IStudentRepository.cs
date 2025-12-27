using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IStudentRepository: IBaseRepository<Student, int>
    {
        Task<Student> GetStudentWithCoursesAsync(int studentId);
        Task<List<Student>> GetAllStudentsWithCoursesAsync();
        Task<Student> GetByEmailAsync(string email);
        Task<IEnumerable<Student>> GetByIdsAsync(List<int> ids);
        void DeleteRange(IEnumerable<Student> students);

    }
}
