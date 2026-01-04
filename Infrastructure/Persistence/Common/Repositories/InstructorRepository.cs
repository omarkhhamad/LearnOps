using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence.Common.Repositories
{
    public class InstructorRepository : BaseRepository<Instructor, int>, IInstructorRepository
    {
        public InstructorRepository(AppDbContext context) : base(context)
        {
        }
        public void DeleteRange(IEnumerable<Instructor> instructors)
        {
         
            foreach (var instructor in instructors)
            {
                instructor.IsDeleted = true;
                instructor.DeletedAt = DateTime.UtcNow;
            }
                _context.Instructors.UpdateRange(instructors);
        }

        public async Task<IEnumerable<Instructor>> GetByIdsAsync(List<int> ids)
        {
           return await _context.Instructors
                .Where(i => ids.Contains(i.InstructorId))
                .ToListAsync();
        }

        public async Task<Instructor?> GetInstructorWithCoursesAndGroupsAsync(int id)
        {
            return await _context.Instructors
                .Include(i => i.ClassGroups)
                    .ThenInclude(g => g.Course)
                .Include(i => i.ClassGroups)
                    .ThenInclude(g => g.Enrollments)
                .FirstOrDefaultAsync(i => i.InstructorId == id);
        }
    }
}
