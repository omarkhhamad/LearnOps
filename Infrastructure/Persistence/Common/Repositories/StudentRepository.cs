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

        public override async Task<IEnumerable<Student>> GetAllAsync()
            => await _context.Students.Include(s => s.User).ToListAsync();

        public override async Task<Student?> GetByIdAsync(int id)
            => await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.StudentId == id);

        public async Task<Student?> GetByEmailAsync(string email)
            => await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.User.Email == email);

        public async Task<IEnumerable<Student>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Students
                .Where(s => ids.Contains(s.StudentId))
                .ToListAsync();
        }



        public async Task<Student?> GetByUserIdAsync(Guid userId)
            => await _context.Students.Include(s => s.User).FirstOrDefaultAsync(s => s.UserId == userId);

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
            => await _context.Students.AnyAsync(s => s.UserId == userId);

        public async Task<Student?> GetStudentWithCoursesAsync(int studentId)
        {
            return await _context.Students
                .Include(s => s.User)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.ClassGroup)
                        .ThenInclude(g => g.Course)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }


    }
}