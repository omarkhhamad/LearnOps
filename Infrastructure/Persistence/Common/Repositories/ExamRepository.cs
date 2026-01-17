using Application.Interfaces.IRepositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .AsNoTracking()
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Course)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Instructor)
                .Include(e => e.ExamResults)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetExamsByGroupIdAsync(int groupId)
        {
            return await _context.Exams
                .AsNoTracking()
                .Where(e => e.GroupId == groupId)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Course)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Instructor)
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetExamsByCourseIdAsync(int courseId)
        {
            return await _context.Exams
                .AsNoTracking()
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Course)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Instructor)
                .Where(e => e.ClassGroup.CourseId == courseId)
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync();
        }

        public async Task<Exam?> GetExamWithResultsAsync(int examId)
        {
            return await _context.Exams
                .AsNoTracking()
                .Include(e => e.ExamResults)
                    .ThenInclude(er => er.Enrollment)
                        .ThenInclude(en => en.Student)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Course)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Instructor)
                .FirstOrDefaultAsync(e => e.ExamId == examId);
        }

        public async Task<bool> ExistInGroupAsync(int groupId, string examTitle)
        {
            if (string.IsNullOrWhiteSpace(examTitle))
            {
                return false;
            }

            return await _context.Exams
                .AsNoTracking()
                .AnyAsync(e =>
                    e.GroupId == groupId &&
                    e.Title.ToLower().Trim() == examTitle.ToLower().Trim()
                );
        }
    }
}