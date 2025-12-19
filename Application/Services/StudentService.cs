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

        public async Task<Result<IEnumerable<StudentDto>>> GetAllStudents()
        {
            var students = await _unitOfWork.Students.GetAllAsync();
            var dtos = students.Select(s => new StudentDto
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                Phone = s.Phone,
                DateOfBirth = s.DateOfBirth
            });

            return Result<IEnumerable<StudentDto>>.Success(dtos);
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

        public async Task<Result<string>> AddStudent(AddUpdateStudentDto studentDto)
        {
            var student = new Student { 
                FullName = studentDto.FullName , 
                Email=studentDto.Email,
                Phone=studentDto.Phone,
                DateOfBirth=studentDto.DateOfBirth,
                CreatedAt=DateTime.UtcNow
            };
            await _unitOfWork.Students.AddAsync(student);
            await _unitOfWork.CommitAsync();
            return Result<string>.Success(null, 200, "Student added successfully");
        }

        public async Task<Result<string>> UpdateStudent(int id, AddUpdateStudentDto studentDto)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null) return Result<string>.Fail("Student not found", 404);

            student.FullName = studentDto.FullName;
            student.Email = studentDto.Email;
            student.Phone = studentDto.Phone;
            student.DateOfBirth = studentDto.DateOfBirth;
            student.CreatedAt = DateTime.UtcNow;

            _unitOfWork.Students.Update(student);
            await _unitOfWork.CommitAsync();
            return Result<string>.Success(null, 200, "Student updated successfully");
        }

        public async Task<Result<string>> DeleteStudent(int id)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null) return Result<string>.Fail("Student not found", 404);
            _unitOfWork.Students.Delete(student);
            await _unitOfWork.CommitAsync();
            return Result<string>.Success(null, 200, "Student deleted successfully");
        }
    }


}
