using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Common.Repositories
{
    public class CourseRepository :BaseRepository<Course,int>, ICourseRepository
    {

        public CourseRepository(AppDbContext context) : base(context)
        {
        }

        public override void Delete(Course course)
        {
            course.IsDeleted = true;
            course.DeletedAt = DateTime.UtcNow;
            _context.Courses.Update(course);
        }

        public void DeleteRange(IEnumerable<Course> courses)
        {
            foreach (var course in courses)
            {
                course.IsDeleted = true;
                course.DeletedAt = DateTime.UtcNow;
            }

            _context.Courses.UpdateRange(courses);
        }

        public async Task<IEnumerable<Course>> GetByIdsAsync(List<int> ids)
        {
            return await _context.Courses
             .Where(s => ids.Contains(s.CourseId))
             .ToListAsync(); 
        }

        public Task<Course> GetCourseWithClassGroupsAsync(int courseId)
        {
            return _context.Courses
                .Where(c => c.CourseId == courseId)
                .Include(c => c.ClassGroups)
                .FirstOrDefaultAsync();
        }
    }
}
