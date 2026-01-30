using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.Instructor;
using Application.Interfaces.IServices;
using Application.Result;
using Application.UnitOfWork;
using AutoMapper;
using Domain.Models;
namespace Application.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public InstructorService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userService = userService;
        }
        public async Task<Result<InstructorDto>> AddInstructor(AddUpdateInstructorDto instructorDto)
        {
            try
            {
                var user = new ApplicationUser
                {
                    FullName = instructorDto.FullName,
                    Email = instructorDto.Email,
                    UserName = instructorDto.Email,
                    PhoneNumber = instructorDto.Phone,
                    CreatedAt = DateTime.UtcNow
                };

                await _userService.CreateUserAsync(user, "Instructor@123", new List<string> { "Instructor" });

                var instructor = await _unitOfWork.Instructors.GetByUserIdAsync(user.Id);
                if (instructor != null)
                {
                    instructor.HourlyRate = instructorDto.HourlyRate;
                    _unitOfWork.Instructors.Update(instructor);
                    await _unitOfWork.CommitAsync();
                }

                var dto = _mapper.Map<InstructorDto>(instructor);
                return Result<InstructorDto>.Success(dto, 201, "Instructor created successfully");
            }
            catch (Exception ex)
            {
                return Result<InstructorDto>.Fail(ex.Message, 400);
            }
        }

        public async Task<Result<bool>> DeleteInstructor(int id)
        {
            try
            {
                var instructor = await _unitOfWork.Instructors.GetByIdAsync(id);
                if (instructor != null)
                {
                    await _userService.DeleteUserAsync(instructor.UserId);
                }
                return Result<bool>.Success(true, 200, "Instructor deleted successfully");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(ex.Message, 400);
            }
        }

        public async Task<Result<bool>> DeleteInstructors(List<int> ids)
        {
            try
            {
                var instructors = await _unitOfWork.Instructors.GetByIdsAsync(ids);
                if (instructors != null && instructors.Any())
                {
                    var userIds = instructors.Select(i => i.UserId).ToList();
                    await _userService.DeleteUsersAsync(userIds);
                }

                return Result<bool>.Success(true, 200, "Instructors deleted successfully");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail(ex.Message, 400);
            }
        }

        public async Task<Result<PagedResult<InstructorDto>>> GetAllInstructors(string? search, int page, int pageSize)
        {
            var instructors = await _unitOfWork.Instructors.GetAllAsync();
            if (!string.IsNullOrEmpty(search))
            {
                instructors = instructors.Where(i => i.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (i.Email != null && i.Email.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                i.Phone.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            var totalRecords = instructors.Count();
            var pagedInstructors = instructors.Skip((page - 1) * pageSize).Take(pageSize);
            var dtos = pagedInstructors.Select(i => new InstructorDto
            {
                InstructorId = i.InstructorId,
                FullName = i.FullName,
                Phone = i.Phone,
                Email = i.Email,
                HourlyRate = i.HourlyRate
            });
            var pagedResult = new PagedResult<InstructorDto>
            {
                Items = dtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
            return Result<PagedResult<InstructorDto>>.Success(pagedResult, 200, "Instructors retrieved successfully");
        }

        public async Task<Result<InstructorDto>> GetInstructorById(int id)
        {
            var instructor = await _unitOfWork.Instructors.GetByIdAsync(id);
            if (instructor == null) return Result<InstructorDto>.Fail("Instructor not found", 404);
            var dto = _mapper.Map<InstructorDto>(instructor);
            return Result<InstructorDto>.Success(dto);
        }

        public async Task<Result<InstructorDto>> UpdateInstructor(int id, AddUpdateInstructorDto instructor)
        {
            var existingInstructor = await _unitOfWork.Instructors.GetByIdAsync(id);
            if (existingInstructor == null)
                return Result<InstructorDto>.Fail("Instructor not found", 404);

            existingInstructor.FullName = instructor.FullName;
            existingInstructor.Phone = instructor.Phone;
            existingInstructor.Email = instructor.Email;
            existingInstructor.HourlyRate = instructor.HourlyRate;

            _unitOfWork.Instructors.Update(existingInstructor);
            await _unitOfWork.CommitAsync();

            var dto = _mapper.Map<InstructorDto>(existingInstructor);

            return Result<InstructorDto>.Success(dto, 200, "Instructor updated successfully");
        }

        public async Task<Result<InstructorDetailedDto>> GetInstructorDetailedById(int id)
        {
            var instructor = await _unitOfWork.Instructors.GetInstructorWithCoursesAndGroupsAsync(id);
            if (instructor == null)
                return Result<InstructorDetailedDto>.Fail("Instructor not found", 404);

            var dto = _mapper.Map<InstructorDetailedDto>(instructor);

            return Result<InstructorDetailedDto>.Success(dto);
        }


    }
}
