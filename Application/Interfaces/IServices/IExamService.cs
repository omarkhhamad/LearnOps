using Application.DTOs.Exam;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices
{
    public interface IExamService
    {
        Task<ExamDTO> CreateExam(ExamDTO exam);
        Task<Application.DTOs.Exam.ExamWithClassGroup?> GetExamById(int id);
        Task<List<ExamWithClassGroup>> GetAllExams();
        Task<ExamDTO> UpdateExam(ExamDTO exam,int id);
        Task<bool> DeleteExam(int id);
        Task<List<ExamWithClassGroup>> GetExamsByGroupIdAsync(int groupId);
        Task<List<Application.DTOs.Exam.ExamWithClassGroup>> GetExamsByCourseIdAsync(int courseId);
        Task<Application.DTOs.Exam.ExamWithClassGroup?> GetExamWithResultsAsync(int examId);
    }
}
