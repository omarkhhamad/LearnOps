using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.ClassGroup;
using Application.DTOs.Instructor;
using Application.Result;
using Domain.Models;
namespace Application.Interfaces.IServices
{
    public interface IClassGroupService
    {
        Task<Result<PagedResult<ClassGroupDto>>> GetAllGroups(string? search, int page, int pageSize);
        Task<Result<ClassGroupDetailedDto>> GetGroupsByIdAsync(int id);
        Task<Result<List<ClassGroupDto>>> GetGroupsByCourseId(int id);
        Task<Result<ClassGroupDto>> AddGroup(AddUpdateClassGroupDto group);
        Task<Result<ClassGroupDto>> UpdateGroup(int id, AddUpdateClassGroupDto group);
        Task<Result<bool>> DeleteGroup(int id);
        Task<Result<bool>> DeleteGroups(List<int> ids);

    }
}
