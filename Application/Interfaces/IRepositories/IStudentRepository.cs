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
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task AddAsync(Student student);
        void Update(Student student);
        void Delete(Student student);
    }
}
