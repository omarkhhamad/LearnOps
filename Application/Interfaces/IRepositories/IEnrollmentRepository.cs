using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IEnrollmentRepository : IBaseRepository<Enrollment, int>
    {
        Task<IEnumerable<Enrollment>> GetEnrollmentsWithDetailsByStudentId(int studentId);
    }
}