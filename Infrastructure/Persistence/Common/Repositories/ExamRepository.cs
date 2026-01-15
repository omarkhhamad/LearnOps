using Application.Interfaces.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Common.Repositories
{
    public class ExamRepository : BaseRepository<Exam, int>, IExamRepository
    {
        public ExamRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Exam>> GetAllExamsAsync()
        {
            return await _context.Exams
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Course)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Instructor)
                .Include(r => r.ExamResults)
                .ToListAsync();
        }
        public async Task<IEnumerable<Exam>> GetExamsByGroupIdAsync(int groupId)
        {
           return await _context.Exams
                .Where(e => e.GroupId == groupId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetExamsByCourseIdAsync(int courseId)
        {
            return await _context.Exams
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Course)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Instructor)
                .Where(e => e.ClassGroup.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<Exam?> GetExamWithResultsAsync(int examId)
        {
            return await _context.Exams
                .Include(e => e.ExamResults)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Course)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Instructor)
                .FirstOrDefaultAsync(e => e.ExamId == examId);
        }
    }
}
