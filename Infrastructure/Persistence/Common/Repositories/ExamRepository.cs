using Application.Interfaces.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Common.Repositories
{
    public class ExamRepository : BaseRepository<Exam, int>, IExamRepository
    {
        public ExamRepository(AppDbContext context) : base(context) { }

        // Helper method to include common navigation properties
        private IQueryable<Exam> IncludeAll(IQueryable<Exam> query)
        {
            return query
                   .Include(e => e.ClassGroup)
                       .ThenInclude(g => g.Course)
                   .Include(e => e.ClassGroup)
                       .ThenInclude(g => g.Instructor)
                            .ThenInclude(i => i.User)
                   .Include(e => e.ExamResults)
                       .ThenInclude(er => er.Student!)
                            .ThenInclude(s => s.User);
        }

        public async Task<IEnumerable<Exam>> GetAllExamsAsync()
        {
            return await IncludeAll(_context.Exams.AsNoTracking())
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetExamsByGroupIdAsync(int groupId)
        {
            return await IncludeAll(_context.Exams.AsNoTracking()
                    .Where(e => e.GroupId == groupId))
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetExamsByCourseIdAsync(int courseId)
        {
            return await IncludeAll(_context.Exams.AsNoTracking()
                    .Where(e => e.ClassGroup.CourseId == courseId))
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync();
        }

        public async Task<Exam?> GetExamWithResultsAsync(int examId)
        {
            return await IncludeAll(_context.Exams.AsNoTracking())
                .FirstOrDefaultAsync(e => e.ExamId == examId);
        }

        public async Task<Exam?> GetExamByIdAsync(int id)
        {
            return await IncludeAll(_context.Exams.AsNoTracking())
                .FirstOrDefaultAsync(e => e.ExamId == id);
        }

        public async Task<bool> ExistInGroupAsync(int groupId, string examTitle)
        {
            if (string.IsNullOrWhiteSpace(examTitle)) return false;

            return await _context.Exams.AsNoTracking()
                .AnyAsync(e =>
                    e.GroupId == groupId &&
                    e.Title.ToLower().Trim() == examTitle.ToLower().Trim());
        }

        public new Task<bool> DeleteRange(IEnumerable<Exam> entities)
        {
            try
            {
                _context.Exams.RemoveRange(entities);
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }
    }
}
