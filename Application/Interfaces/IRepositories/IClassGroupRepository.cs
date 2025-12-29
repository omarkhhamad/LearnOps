using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;
namespace Application.Interfaces.IRepositories
{
    public interface IClassGroupRepository:IBaseRepository<ClassGroup,int>
    {
        Task<IEnumerable<ClassGroup>> GetAllWithRelatedDataAsync();
        Task<IEnumerable<ClassGroup>> GetByIdsAsync(List<int> ids);
        Task<ClassGroup?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<ClassGroup>> GetGroupsByCourseIdWithDetails(int courseId);
    }
}
