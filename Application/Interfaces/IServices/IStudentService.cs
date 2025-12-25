using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Student;
using Application.Result;
using Domain.Models;
namespace Application.Interfaces.IServices
{
    public interface IStudentService
    {
        Task<Result<PagedResult<StudentDto>>> GetAllStudents(string? search, int page, int pageSize);
        Task<Result<StudentDto>> GetStudentById(int id);
        Task<Result<StudentDto>> AddStudent(AddUpdateStudentDto studentDto);
        Task<Result<StudentDto>> UpdateStudent(int id, AddUpdateStudentDto studentDto);
        Task<Result<bool>> DeleteStudent(int id);
    }
}
