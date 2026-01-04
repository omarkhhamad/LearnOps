using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence.Common.Repositories
{
    public class ClassGroupRepository : BaseRepository<ClassGroup, int>, IClassGroupRepository
    {

        public ClassGroupRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ClassGroup>> GetAllWithRelatedDataAsync()
        {
            return await _context.ClassGroups
                .Include(cg => cg.Course)
                .Include(cg => cg.Instructor)
                .Include(cg => cg.Enrollments)
                    .ThenInclude(e => e.Student)
                .Where(cg => !cg.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClassGroup>> GetByIdsAsync(List<int> ids)
        {
            return await _context.ClassGroups
                .Where(cg => ids.Contains(cg.GroupId))
                .Include(cg => cg.Course)
                 .Include(cg => cg.Instructor)
                   .Include(cg => cg.Enrollments)
                    .ThenInclude(e => e.Student)
                .ToListAsync();
        }

        public async Task<ClassGroup?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.ClassGroups
                .Include(cg => cg.Course)
                .Include(cg => cg.Instructor)
                .Include(cg => cg.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(cg => cg.GroupId == id && !cg.IsDeleted);
        }



        public async Task<IEnumerable<ClassGroup>> GetGroupsByCourseIdWithDetails(int courseId)
        {
            return await _context.ClassGroups
                .Include(g => g.Instructor)
                .Include(g => g.Enrollments)
                    .ThenInclude(e => e.Student)
                .Where(g => g.CourseId == courseId)
                .ToListAsync();
        }

    }
}
