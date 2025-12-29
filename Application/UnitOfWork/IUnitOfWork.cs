using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;

namespace Application.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentRepository Students { get; }
        ICourseRepository Courses { get; }
        IInstructorRepository Instructors { get; }
        IClassGroupRepository ClassGroups { get; }
        IEnrollmentRepository Enrollments { get; }
        Task<int> CommitAsync();
    }

}
