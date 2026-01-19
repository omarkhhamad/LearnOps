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

        //public async Task<IEnumerable<Exam>> GetAllExamsAsync()
        //{
        //    return await _context.Exams
        //        .AsNoTracking()
        //        .Select(e => new Exam
        //        {
        //            ExamId = e.ExamId,
        //            Title = e.Title,
        //            ExamDate = e.ExamDate,
        //            MaxScore = e.MaxScore,
        //            GroupId = e.GroupId,
        //            ClassGroup = new ClassGroup
        //            {
        //                GroupId = e.ClassGroup.GroupId,
        //                Name = e.ClassGroup.Name,
        //                Room = e.ClassGroup.Room,
        //                Days = e.ClassGroup.Days,
        //                Time = e.ClassGroup.Time,
        //                StartDate = e.ClassGroup.StartDate,
        //                EndDate = e.ClassGroup.EndDate,
        //                CourseId = e.ClassGroup.CourseId,
        //                InstructorId = e.ClassGroup.InstructorId,
        //                Course = e.ClassGroup.Course,
        //                Instructor = e.ClassGroup.Instructor
        //            },
        //            ExamResults = e.ExamResults
        //                .Select(er => new ExamResult
        //                {
        //                    ExamResultId = er.ExamResultId,
        //                    ExamId = er.ExamId,
        //                    EnrollmentId = er.EnrollmentId,
        //                    Score = er.Score,
        //                    Enrollment = new Enrollment
        //                    {
        //                        EnrollmentId = er.Enrollment.EnrollmentId,
        //                        StudentId = er.Enrollment.StudentId,
        //                        Student = er.Enrollment.Student
        //                    }
        //                }).ToList()
        //        })
        //        .ToListAsync();
        //}

        public async Task<IEnumerable<Exam>> GetAllExamsAsync()
        {
            return await _context.Exams
                .AsNoTracking()
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Course)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Instructor)
                .Include(e => e.ExamResults)
                    .ThenInclude(er => er.Enrollment)
                        .ThenInclude(en => en.Student)
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
                .Include(e => e.ExamResults)
                    .ThenInclude(er => er.Enrollment)
                        .ThenInclude(en => en.Student)
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
                .Include(e => e.ExamResults)
                    .ThenInclude(er => er.Enrollment)
                        .ThenInclude(en => en.Student)
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
                .Include(e => e.ExamResults)
                    .ThenInclude(er => er.Enrollment)
                        .ThenInclude(en => en.Student)
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
        public async Task<Exam?> GetExamByIdAsync(int id)
        {
            return await _context.Exams
                .AsNoTracking()
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Course)
                .Include(e => e.ClassGroup)
                    .ThenInclude(g => g.Instructor)
                .Include(e => e.ExamResults)
                    .ThenInclude(er => er.Enrollment)
                        .ThenInclude(en => en.Student)
                .FirstOrDefaultAsync(e => e.ExamId == id);
        }

        public Task<bool> DeleteRange(IEnumerable<Exam> entities)
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