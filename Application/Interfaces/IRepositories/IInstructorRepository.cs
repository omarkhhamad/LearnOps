using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IInstructorRepository:IBaseRepository<Instructor,int>
    {
        Task<Instructor> GetInstructorWithClassGroupsAsync(int id);
        Task<IEnumerable<Instructor>> GetByIdsAsync(List<int> ids);
        void DeleteRange(IEnumerable<Instructor> students);
    }
}
