using System;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentRepository Students { get; }
        ICourseRepository Courses { get; }
        IInstructorRepository Instructors { get; }
        IClassGroupRepository ClassGroups { get; }
        IEnrollmentRepository Enrollments { get; }
        IExamRepository Exams { get; }
        IUserRepository Users { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        Task<int> CommitAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        IExecutionStrategy CreateExecutionStrategy();
    }
}
