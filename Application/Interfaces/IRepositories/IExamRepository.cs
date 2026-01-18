using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IRepositories
{
    public interface IExamRepository : IBaseRepository <Exam, int>
    {
        Task<IEnumerable<Exam>> GetAllExamsAsync();
        Task<IEnumerable<Exam>> GetExamsByGroupIdAsync(int groupId);
        Task<IEnumerable<Exam>> GetExamsByCourseIdAsync(int courseId);
        Task<Exam?> GetExamWithResultsAsync(int examId);
        Task<bool> ExistInGroupAsync(int GroupId , string CourseName);
        Task<Exam> GetExamByIdAsync(int Id);
        Task<bool> DeleteRange(IEnumerable<Exam> entities);
    }
}
