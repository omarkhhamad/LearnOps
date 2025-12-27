using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence.Common.Repositories
{
    public class StudentRepository : BaseRepository<Student, int>, IStudentRepository
    {
        private new readonly AppDbContext _context;

        public StudentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Student> GetStudentWithCoursesAsync(int studentId)
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.ClassGroup)
                        .ThenInclude(cg => cg.Course)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        public async Task<List<Student>> GetAllStudentsWithCoursesAsync()
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.ClassGroup)
                        .ThenInclude(cg => cg.Course)
                .ToListAsync();
        }

        public async Task<Student> GetByEmailAsync(string email)
            => await _context.Students.FirstOrDefaultAsync(s => s.Email == email);

        public async Task<IEnumerable<Student>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Students
                .Where(s => ids.Contains(s.StudentId))
                .ToListAsync();
        }

        //public void DeleteRange(IEnumerable<Student> students)
        //{
        //    _context.Students.RemoveRange(students);
        //}

        public new void Delete(Student student)
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

    }
}