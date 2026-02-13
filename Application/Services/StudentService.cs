using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Student;
using Application.Interfaces.IServices;
using Application.Bases;
using Application.UnitOfWork;
using AutoMapper;
using Domain.Models;
namespace Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public StudentService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<Result<PagedResult<StudentDto>>> GetAllStudents(string? search, int page = 1, int pageSize = 10)
        {
            var students = await _unitOfWork.Students.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                students = students.Where(s =>
                    (s.User != null && s.User.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (s.User != null && s.User.Email != null && s.User.Email.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (s.User != null && s.User.PhoneNumber != null && s.User.PhoneNumber.Contains(search, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }
            var totalRecords = students.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            var pagedStudents = students.Skip((page - 1) * pageSize).Take(pageSize);
            var dtos = _mapper.Map<IEnumerable<StudentDto>>(pagedStudents);

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

            var dto = _mapper.Map<StudentDto>(student);
            return Result<StudentDto>.Success(dto);
        }
        public async Task<Result<StudentDto>> AddStudent(AddUpdateStudentDto studentDto)
        {
            try
            {
                var user = new ApplicationUser
                {
                    FullName = studentDto.FullName,
                    UserName = studentDto.UserName,
                    Email = studentDto.Email,
                    PhoneNumber = studentDto.Phone,
                    CreatedAt = DateTime.UtcNow
                };

                await _userService.CreateUserAsync(user, "Student@123", new List<string> { "Student" });

                var student = await _unitOfWork.Students.GetByUserIdAsync(user.Id);
                if (student != null)
                {
                    student.DateOfBirth = studentDto.DateOfBirth;
                    _unitOfWork.Students.Update(student);
                    await _unitOfWork.CommitAsync();
                }

                var dto = _mapper.Map<StudentDto>(student);
                return Result<StudentDto>.Success(dto, 201, "Student added successfully");
            }
            catch (Exception ex)
            {
                return Result<StudentDto>.Fail(ex.Message, 400);
            }
        }

        public async Task<Result<StudentDto>> UpdateStudent(int id, AddUpdateStudentDto studentDto)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(id);
            if (student == null) return Result<StudentDto>.Fail("Student not found", 404);

            // Update ApplicationUser fields
            var userResult = await _userService.GetByIdAsync(student.UserId);
            if (userResult.IsSuccess && userResult.Data != null)
            {
                var userDto = userResult.Data;
                userDto.FullName = studentDto.FullName;
                userDto.UserName = studentDto.UserName;
                userDto.Email = studentDto.Email ?? userDto.Email;
                userDto.PhoneNumber = studentDto.Phone;
                await _userService.UpdateUserAsync(student.UserId, userDto);
            }

            // Update Student profile fields
            student.DateOfBirth = studentDto.DateOfBirth;
            _unitOfWork.Students.Update(student);
            await _unitOfWork.CommitAsync();

            var dto = _mapper.Map<StudentDto>(student);

            return Result<StudentDto>.Success(dto, 200, "Student updated successfully");
        }

        public async Task<Result<bool>> DeleteStudent(int id)
        {
            try
            {
                var student = await _unitOfWork.Students.GetByIdAsync(id);
                if (student != null)
                {
                    await _userService.DeleteUserAsync(student.UserId);
                }
                return Result<bool>.Success(true, 200, "Student deleted successfully");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(ex.Message, 400);
            }
        }

        public async Task<Result<bool>> DeleteStudents(List<int> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                    return Result<bool>.Fail("No students selected", 400);

                var students = await _unitOfWork.Students.GetByIdsAsync(ids);
                if (students.Any())
                {
                    var userIds = students.Select(s => s.UserId).ToList();
                    await _userService.DeleteUsersAsync(userIds);
                }

                return Result<bool>.Success(true, 200, $"{students.Count()} students deleted successfully");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(ex.Message, 400);
            }
        }

        public async Task<Result<StudentDetailedDto>> GetStudentDetailedById(int id)
        {
            var student = await _unitOfWork.Students.GetStudentWithCoursesAsync(id);
            if (student == null)
                return Result<StudentDetailedDto>.Fail("Student not found", 404);

            var dto = _mapper.Map<StudentDetailedDto>(student);

            return Result<StudentDetailedDto>.Success(dto);
        }

    }


}
