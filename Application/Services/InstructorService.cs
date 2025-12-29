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
        public InstructorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<InstructorDto>> AddInstructor(AddUpdateInstructorDto instructor)
        {
            var newInstructor = _mapper.Map <Instructor>(instructor);
            await _unitOfWork.Instructors.AddAsync(newInstructor);
            await _unitOfWork.CommitAsync();

            var dto = _mapper.Map <InstructorDto>(newInstructor);

            return Result<InstructorDto>.Success(dto, 201, "Instructor created successfully");
        }

        public async Task<Result<bool>> DeleteInstructor(int id)
        {
            var instructor = await  _unitOfWork.Instructors.GetByIdAsync(id);
            if (instructor == null)
            {
                return Result<bool>.Fail("Instructor not found", 404);
            }
            _unitOfWork.Instructors.Delete(instructor);
            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, 200, "Instructor deleted successfully");
        }

        public async Task<Result<bool>> DeleteInstructors(List<int> ids)
        {
            var instructors = await _unitOfWork.Instructors.GetByIdsAsync(ids);
            if (instructors == null || !instructors.Any())
            {
                return Result<bool>.Fail("Instructors not found", 404);
            }
            _unitOfWork.Instructors.DeleteRange(instructors);
            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, 200, "Instructors deleted successfully");
        }

        public async Task<Result<PagedResult<InstructorDto>>> GetAllInstructors(string? search, int page, int pageSize)
        {
            var instructors =await  _unitOfWork.Instructors.GetAllAsync();
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
            var instructor = await  _unitOfWork.Instructors.GetByIdAsync(id);
            if (instructor == null) return Result<InstructorDto>.Fail("Instructor not found", 404);
            var dto = _mapper.Map <InstructorDto>(instructor);
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

            var dto = _mapper.Map <InstructorDto>(existingInstructor);

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
