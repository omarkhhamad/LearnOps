using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Course;
using Application.DTOs.Student;
using Application.Result;

namespace Application.Interfaces.IServices
{
    public interface ICourseService
    {
        Task<Result<PagedResult<CourseDto>>> GetAllCourses(string? search, int page, int pageSize);
        Task<Result<CourseDto>> GetCourseById(int id);
        Task<Result<CourseDto>> AddCourse(AddUpdateCourseDto courseDto);
        Task<Result<CourseDto>> UpdateCourse(int id, AddUpdateCourseDto courseDto);
        Task<Result<bool>> DeleteCourse(int id);
        Task<Result<bool>> DeleteCourses(List<int> ids);
    }
}
