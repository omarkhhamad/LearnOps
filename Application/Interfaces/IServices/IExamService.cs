using Application.DTOs.Exam;
using Application.Bases;
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
        Task<Result<ExamDto>> CreateExam(ExamDto exam);
        Task<Result<ExamWithClassGroupDto?>> GetExamById(int id);
        Task<Result<PagedResult<ExamWithClassGroupDto>>> GetAllExams(string? Search, int Page, int PageSize);
        Task<Result<ExamDto>> UpdateExam(ExamDto exam,int id);
        Task<Result<bool>> DeleteExam(int id);
        Task<Result<List<ExamWithClassGroupDto>>> GetExamsByGroupIdAsync(int groupId);
        Task<Result<List<ExamWithClassGroupDto>>> GetExamsByCourseIdAsync(int courseId);
        Task<Result<ExamWithClassGroupDto?>> GetExamWithResultsAsync(int examId);
        Task<Result<bool>> DeleteRangeOfExams(int[] examsIds);
    }
}
