using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Course;
using Application.Interfaces.IServices;
using Application.Result;
using Application.UnitOfWork;
namespace Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CourseDto>> AddCourse(AddUpdateCourseDto courseDto)
        {
            var course = new Domain.Models.Course
            {
                Title = courseDto.Title,
                Description = courseDto.Description,
                DurationWeeks = courseDto.DurationWeeks,
                Price = courseDto.Price,
                MaxStudents = courseDto.MaxStudents
            };
            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.CommitAsync();
            var resultDto = new CourseDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                DurationWeeks = course.DurationWeeks,
                Price = course.Price,
                MaxStudents = course.MaxStudents
            };
            return Result<CourseDto>.Success(resultDto, 201, "Course created successfully");
        }

        public async Task<Result<bool>> DeleteCourse(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
            {
                return Result<bool>.Fail("Course not found", 404);
            }
            _unitOfWork.Courses.Delete(course);
            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, 200, "Course deleted successfully");
        }

        public async Task<Result<bool>> DeleteCourses(List<int> ids)
        {
            var courses = await _unitOfWork.Courses.GetByIdsAsync(ids);
            if (courses == null || !courses.Any())
            {
                return Result<bool>.Fail("No courses found to delete", 404);
            }
            _unitOfWork.Courses.DeleteRange(courses);
            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, 200, $"{courses.Count()} courses deleted successfully");
        }

        public async Task<Result<PagedResult<CourseDto>>> GetAllCourses(string? search, int page, int pageSize)
        {
            var courses = await _unitOfWork.Courses.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                courses = courses.Where(c => c.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (c.Description != null && c.Description.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }
            var totalRecords = courses.Count();
            var pagedCourses = courses.Skip((page - 1) * pageSize).Take(pageSize);
            var dtos = pagedCourses.Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                Title = c.Title,
                Description = c.Description,
                DurationWeeks = c.DurationWeeks,
                Price = c.Price,
                MaxStudents = c.MaxStudents
            });
            var pagedResult = new PagedResult<CourseDto>
            {
                Items = dtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
            return Result<PagedResult<CourseDto>>.Success(pagedResult);
        }

        public async Task<Result<CourseDto>> GetCourseById(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
            {
                return Result<CourseDto>.Fail("Course not found", 404);
            }
            var dto = new CourseDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                DurationWeeks = course.DurationWeeks,
                Price = course.Price,
                MaxStudents = course.MaxStudents
            };
            return Result<CourseDto>.Success(dto);
        }

        public async Task<Result<CourseDto>> UpdateCourse(int id, AddUpdateCourseDto courseDto)
        {
              var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                 return Result<CourseDto>.Fail("Course not found", 404);
            course.Title = courseDto.Title; 
            course.Description = courseDto.Description;
            course.DurationWeeks = courseDto.DurationWeeks;
            course.Price = courseDto.Price;
            course.MaxStudents = courseDto.MaxStudents;
            _unitOfWork.Courses.Update(course);

            await _unitOfWork.CommitAsync();
            var dto = new CourseDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                DurationWeeks = course.DurationWeeks,
                Price = course.Price,
                MaxStudents = course.MaxStudents
            };
            return Result<CourseDto>.Success(dto, 200, "Course updated successfully");
        }
    }
}
