using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence.Common.Repositories
{
    public class StudentRepository : BaseRepository<Student, int>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Student> GetByEmailAsync(string email)
            => await _context.Students.FirstOrDefaultAsync(s => s.Email == email);

        public async Task<IEnumerable<Student>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Students
                .Where(s => ids.Contains(s.StudentId))
                .ToListAsync();
        }

        public override void Delete(Student student)
        {
            student.IsDeleted = true;
            student.DeletedAt = DateTime.UtcNow;
            _context.Students.Update(student);
        }

        public void DeleteRange(IEnumerable<Student> students)
        {
            foreach (var student in students)
            {
                student.IsDeleted = true;
                student.DeletedAt = DateTime.UtcNow;
            }

            _context.Students.UpdateRange(students);
        }

        public async Task<Student?> GetStudentWithCoursesAsync(int studentId)
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.ClassGroup)
                        .ThenInclude(g => g.Course)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }


    }
}