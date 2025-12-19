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
        Task<Result<IEnumerable<StudentDto>>> GetAllStudents();
        Task<Result<StudentDto>> GetStudentById(int id);
        Task<Result<string>> AddStudent(AddUpdateStudentDto studentDto);
        Task<Result<string>> UpdateStudent(int id, AddUpdateStudentDto studentDto);
        Task<Result<string>> DeleteStudent(int id);
    }
}
