using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Student;
using Application.Interfaces;
using Application.Interfaces.IServices;
using Application.Result;
using Application.UnitOfWork;
using Domain.Models;
namespace Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public async Task<Result<IEnumerable<StudentDto>>> GetAllStudents()
        //{
        //    var students = await _unitOfWork.Students.GetAllAsync();
        //    var dtos = students.Select(s => new StudentDto
        //    {
        //        StudentId = s.StudentId,
        //        FullName = s.FullName,
        //        Email = s.Email,
        //        Phone = s.Phone,
        //        DateOfBirth = s.DateOfBirth
        //    });

        //    return Result<IEnumerable<StudentDto>>.Success(dtos);
        //}
        public async Task<Result<PagedResult<StudentDto>>> GetAllStudents(string? search,int page=1,int pageSize=10)
        {
            var students = await _unitOfWork.Students.GetAllAsync();
           if(!string.IsNullOrEmpty(search))
            {
                students = students.Where(s => s.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) || 
                (s.Email != null && s.Email.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                s.Phone.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var totalRecords = students.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            var pagedStudents = students.Skip((page - 1) * pageSize).Take(pageSize);
            var dtos = pagedStudents.Select(s => new StudentDto
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                Phone = s.Phone,
                DateOfBirth = s.DateOfBirth
            });
            var pagedResult = new PagedResult<StudentDto>
            {
                Items = dtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            return Result<PagedResult<StudentDto>>.Success(pagedResult, 200, $"Page {page} of {pagedResult.TotalPages}, Total Records: {totalRecords}");
        }



        public async Task<Result<StudentDto>> GetStudentById(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null) return Result<StudentDto>.Fail("Student not found", 404);

            var dto = new StudentDto {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email=student.Email, 
                DateOfBirth=student.DateOfBirth,
                Phone=student.Phone,
            };
            return Result<StudentDto>.Success(dto);
        }
        public async Task<Result<StudentDto>> AddStudent(AddUpdateStudentDto studentDto)
        {
            var student = new Student
            {
                FullName = studentDto.FullName,
                Email = studentDto.Email,
                Phone = studentDto.Phone,
                DateOfBirth = studentDto.DateOfBirth,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.CommitAsync();

            var dto = new StudentDto
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                Phone = student.Phone,
                DateOfBirth = student.DateOfBirth
            };

            return Result<StudentDto>.Success(dto, 201, "Student added successfully");
        }

        public async Task<Result<StudentDto>> UpdateStudent(int id, AddUpdateStudentDto studentDto)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null) return Result<StudentDto>.Fail("Student not found", 404);

            student.FullName = studentDto.FullName;
            student.Email = studentDto.Email;
            student.Phone = studentDto.Phone;
            student.DateOfBirth = studentDto.DateOfBirth;
            student.CreatedAt = DateTime.UtcNow;

            _unitOfWork.Students.Update(student);
            await _unitOfWork.CommitAsync();

            var dto = new StudentDto
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                Phone = student.Phone,
                DateOfBirth = student.DateOfBirth
            };

            return Result<StudentDto>.Success(dto, 200, "Student updated successfully");
        }

        public async Task<Result<bool>> DeleteStudent(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null) return Result<bool>.Fail("Student not found", 404);

            _unitOfWork.Students.Delete(student);
            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, 200, "Student deleted successfully");
        }

        public async Task<Result<bool>> DeleteStudents(List<int> ids)
        {
            if (ids == null || !ids.Any())
                return Result<bool>.Fail("No students selected", 400);

            var students = await _unitOfWork.Students.GetByIdsAsync(ids);

            if (!students.Any())
                return Result<bool>.Fail("No students found", 404);

            _unitOfWork.Students.DeleteRange(students);
            await _unitOfWork.CommitAsync();

            return Result<bool>.Success(true, 200, $"{students.Count()} students deleted successfully");
        }


    }


}
