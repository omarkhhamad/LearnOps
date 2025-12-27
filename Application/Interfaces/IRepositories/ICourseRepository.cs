using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
namespace Application.Interfaces.IRepositories
{
    public interface ICourseRepository: IBaseRepository<Course, int>
    {
        Task<Course> GetCourseWithClassGroupsAsync(int courseId);
        Task<IEnumerable<Course>> GetByIdsAsync(List<int> ids);
        void DeleteRange(IEnumerable<Course> courses);
    }
}
