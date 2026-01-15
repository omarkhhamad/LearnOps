using Application.DTOs.ClassGroup;
using Application.DTOs.Exam;
using Application.Interfaces.IServices;
using Application.UnitOfWork;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExamService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ExamDTO> CreateExam(ExamDTO exam)
        {
            var newExam = new Exam
            {
                Title = exam.Title,
                ExamDate = exam.ExamDate,
                MaxScore = exam.MaxScore,
                GroupId = exam.GroupId
            };
            await _unitOfWork.Exams.AddAsync(newExam);
            await _unitOfWork.CommitAsync();
            return exam;
        }

        public async Task<bool> DeleteExam(int id)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
            {
                throw new ValidationException("Exam not found");
            }
            _unitOfWork.Exams.Delete(exam);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<List<ExamWithClassGroup>> GetAllExams()
        {
            var exams = await _unitOfWork.Exams.GetAllExamsAsync();
            var examDtos = exams.Select(e => new ExamWithClassGroup
            {
                ExamId = e.ExamId,
                Title = e.Title,
                ExamDate = e.ExamDate,
                MaxScore = e.MaxScore,
                GroupId = e.GroupId,
                ClassGroup = new ClassGroupDto
                {
                    GroupId = e.ClassGroup.GroupId,
                    Name = e.ClassGroup.Name,
                    Room = e.ClassGroup.Room,
                    Days = e.ClassGroup.Days,
                    Time = e.ClassGroup.Time,
                    StartDate = e.ClassGroup.StartDate,
                    EndDate = e.ClassGroup.EndDate,
                    CourseName = e.ClassGroup.Course.Title,
                    InstructorName = e.ClassGroup.Instructor.FullName
                },
                ExamResults = e.ExamResults.Select(r => new ClassGroupExamResult
                {
                    Score = r.Score,
                    Result = r.Result
                }).ToList()
            }).ToList();

            return examDtos;
        }

        public async Task<ExamWithClassGroup?> GetExamById(int id)
        {
            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
            {
                return null;
            }

            // project to DTO to avoid cycles
            var dto = new ExamWithClassGroup
            {
                ExamId = exam.ExamId,
                Title = exam.Title,
                ExamDate = exam.ExamDate,
                MaxScore = exam.MaxScore,
                GroupId = exam.GroupId,
                ClassGroup = exam.ClassGroup != null ? new ClassGroupDto
                {
                    GroupId = exam.ClassGroup.GroupId,
                    Name = exam.ClassGroup.Name,
                    Room = exam.ClassGroup.Room,
                    Days = exam.ClassGroup.Days,
                    Time = exam.ClassGroup.Time,
                    StartDate = exam.ClassGroup.StartDate,
                    EndDate = exam.ClassGroup.EndDate,
                    CourseName = exam.ClassGroup.Course?.Title ?? string.Empty,
                    InstructorName = exam.ClassGroup.Instructor?.FullName ?? string.Empty
                } : null,
                ExamResults = exam.ExamResults?.Select(r => new ClassGroupExamResult
                {
                    Score = r.Score,
                    Result = r.Result
                }).ToList() ?? new List<ClassGroupExamResult>()
            };

            return dto;
        }

        public async Task<List<ExamWithClassGroup>> GetExamsByCourseIdAsync(int courseId)
        {
            var exams = await _unitOfWork.Exams.GetExamsByCourseIdAsync(courseId);
            if (exams == null || !exams.Any())
            {
                return new List<ExamWithClassGroup>();
            }

            var list = exams.Select(e => new ExamWithClassGroup
            {
                ExamId = e.ExamId,
                Title = e.Title,
                ExamDate = e.ExamDate,
                MaxScore = e.MaxScore,
                GroupId = e.GroupId,
                ClassGroup = e.ClassGroup != null ? new ClassGroupDto
                {
                    GroupId = e.ClassGroup.GroupId,
                    Name = e.ClassGroup.Name,
                    Room = e.ClassGroup.Room,
                    Days = e.ClassGroup.Days,
                    Time = e.ClassGroup.Time,
                    StartDate = e.ClassGroup.StartDate,
                    EndDate = e.ClassGroup.EndDate,
                    CourseName = e.ClassGroup.Course?.Title ?? string.Empty,
                    InstructorName = e.ClassGroup.Instructor?.FullName ?? string.Empty
                } : null,
                ExamResults = e.ExamResults?.Select(r => new ClassGroupExamResult
                {
                    Score = r.Score,
                    Result = r.Result
                }).ToList() ?? new List<ClassGroupExamResult>()
            }).ToList();

            return list;
        }

        public async Task<List<ExamWithClassGroup>> GetExamsByGroupIdAsync(int groupId)
        {
            var exams = await _unitOfWork.Exams.GetExamsByGroupIdAsync(groupId);
            if (exams == null || !exams.Any())
            {
                return new List<ExamWithClassGroup>();
            }

            var list = exams.Select(e => new ExamWithClassGroup
            {
                ExamId = e.ExamId,
                Title = e.Title,
                ExamDate = e.ExamDate,
                MaxScore = e.MaxScore,
                GroupId = e.GroupId,
                ClassGroup = e.ClassGroup != null ? new ClassGroupDto
                {
                    GroupId = e.ClassGroup.GroupId,
                    Name = e.ClassGroup.Name,
                    Room = e.ClassGroup.Room,
                    Days = e.ClassGroup.Days,
                    Time = e.ClassGroup.Time,
                    StartDate = e.ClassGroup.StartDate,
                    EndDate = e.ClassGroup.EndDate,
                    CourseName = e.ClassGroup.Course?.Title ?? string.Empty,
                    InstructorName = e.ClassGroup.Instructor?.FullName ?? string.Empty
                } : null,
                ExamResults = e.ExamResults?.Select(r => new ClassGroupExamResult
                {
                    Score = r.Score,
                    Result = r.Result
                }).ToList() ?? new List<ClassGroupExamResult>()
            }).ToList();

            return list;
        }

        public async Task<ExamWithClassGroup?> GetExamWithResultsAsync(int examId)
        {
            var exam = await _unitOfWork.Exams.GetExamWithResultsAsync(examId);
            if (exam == null)
            {
                return null;
            }

            var dto = new ExamWithClassGroup
            {
                ExamId = examId,
                Title = exam.Title,
                ExamDate = exam.ExamDate,
                MaxScore = exam.MaxScore,
                GroupId = exam.GroupId,
                ClassGroup = exam.ClassGroup != null ? new ClassGroupDto
                {
                    GroupId = exam.ClassGroup.GroupId,
                    Name = exam.ClassGroup.Name,
                    Room = exam.ClassGroup.Room,
                    Days = exam.ClassGroup.Days,
                    Time = exam.ClassGroup.Time,
                    StartDate = exam.ClassGroup.StartDate,
                    EndDate = exam.ClassGroup.EndDate,
                    CourseName = exam.ClassGroup.Course?.Title ?? string.Empty,
                    InstructorName = exam.ClassGroup.Instructor?.FullName ?? string.Empty
                } : new ClassGroupDto
                {
                    GroupId = 0,
                    Name = string.Empty,
                    Room = string.Empty,
                    Days = string.Empty,
                    Time = string.Empty,
                    StartDate = DateTime.MinValue,
                    EndDate = null,
                    CourseName = string.Empty,
                    InstructorName = string.Empty
                },
                ExamResults = exam.ExamResults?.Select(r => new ClassGroupExamResult
                {
                    Score = r.Score,
                    Result = r.Result
                }).ToList() ?? new List<ClassGroupExamResult>()
            };

            return dto;
        }

        public async Task<ExamDTO> UpdateExam(ExamDTO exam, int id)
        {
            var existExam =await _unitOfWork.Exams.GetByIdAsync(id);
            if (existExam == null)
            {
                throw new ValidationException("Exam not found");
            }
            existExam.Title = exam.Title;
            existExam.ExamDate = exam.ExamDate;
            existExam.MaxScore = exam.MaxScore;
            existExam.GroupId = exam.GroupId;
            _unitOfWork.Exams.Update(existExam);
            await _unitOfWork.CommitAsync();
            return exam;
        }
    }
}
