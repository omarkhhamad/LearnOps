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
    public class InstructorRepository : BaseRepository<Instructor, int>, IInstructorRepository
    {
        private new readonly AppDbContext _context;
        public InstructorRepository(AppDbContext context) : base(context)
        {
            _context = context;
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

        public Task<Instructor> GetInstructorWithClassGroupsAsync(int id)
        {
            return _context.Instructors
                .Include(i => i.ClassGroups)
                .ThenInclude(cg => cg.Course)
                .FirstOrDefaultAsync(i => i.InstructorId == id);
        }
    }
}
