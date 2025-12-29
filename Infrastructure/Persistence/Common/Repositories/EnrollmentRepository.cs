using Application.Interfaces.IRepositories;
using Domain.Models;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence.Common.Repositories
{
    public class EnrollmentRepository : BaseRepository<Enrollment, int>, IEnrollmentRepository
    {
        public EnrollmentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsWithDetailsByStudentId(int studentId)
        {
            return await _context.Enrollments
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Course)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Instructor)
                .Where(e => e.StudentId == studentId)
                .ToListAsync();
        }
    }
}