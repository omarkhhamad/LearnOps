using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Result;
using Domain.Models;
using Application.DTOs.Instructor;
namespace Application.Interfaces.IServices
{
    public interface IInstructorService
    {
        //Task<bool> IsInstructorAvailableAsync(int instructorId, DateTime startTime, DateTime endTime);
        Task<Result<PagedResult<InstructorDto>>> GetAllInstructors(string? search, int page, int pageSize);
        Task<Result<InstructorDto>> GetInstructorById(int id);
        Task<Result<InstructorDto>> AddInstructor(AddUpdateInstructorDto instructor);
        Task<Result<InstructorDto>> UpdateInstructor(int id, AddUpdateInstructorDto instructor);
        Task<Result<bool>> DeleteInstructor(int id);
        Task<Result<bool>> DeleteInstructors(List<int> ids);

    }
}
