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

        public override async Task<IEnumerable<Instructor>> GetAllAsync()
            => await _context.Instructors.Include(i => i.User).ToListAsync();

        public override async Task<Instructor?> GetByIdAsync(int id)
            => await _context.Instructors.Include(i => i.User).FirstOrDefaultAsync(i => i.InstructorId == id);


        public async Task<IEnumerable<Instructor>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Instructors
                 .Where(i => ids.Contains(i.InstructorId))
                 .ToListAsync();
        }

        public async Task<Instructor?> GetByUserIdAsync(Guid userId)
            => await _context.Instructors.Include(i => i.User).FirstOrDefaultAsync(i => i.UserId == userId);

        public async Task<bool> ExistsByUserIdAsync(Guid userId)
            => await _context.Instructors.AnyAsync(i => i.UserId == userId);

        public async Task<Instructor?> GetInstructorWithCoursesAndGroupsAsync(int id)
        {
            return await _context.Instructors
                .Include(i => i.User)
                .Include(i => i.ClassGroups)
                    .ThenInclude(g => g.Course)
                .Include(i => i.ClassGroups)
                    .ThenInclude(g => g.Enrollments)
                .FirstOrDefaultAsync(i => i.InstructorId == id);
        }
    }
}
