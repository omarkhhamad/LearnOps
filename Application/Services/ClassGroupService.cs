using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.ClassGroup;
using Application.DTOs.Course;
using Application.DTOs.Instructor;
using Application.DTOs.Student;
using Application.Interfaces.IServices;
using Application.Result;
using Application.UnitOfWork;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class ClassGroupService : IClassGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ClassGroupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result<ClassGroupDto>> AddGroup(AddUpdateClassGroupDto group)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(group.CourseId);
            if (course == null)
                return Result<ClassGroupDto>.Fail("Course not found", 400);

            var instructor = await _unitOfWork.Instructors.GetByIdAsync(group.InstructorId);
            if (instructor == null)
                return Result<ClassGroupDto>.Fail("Instructor not found", 400);

            var classGroup = _mapper.Map<Domain.Models.ClassGroup>(group);

            await _unitOfWork.ClassGroups.AddAsync(classGroup);
            await _unitOfWork.CommitAsync();

            var createdGroup = await _unitOfWork.ClassGroups.GetByIdAsync(classGroup.GroupId);

            var classGroupDto = _mapper.Map<ClassGroupDto>(createdGroup);

            return Result<ClassGroupDto>.Success(classGroupDto, 201, "Class group created successfully");
        }

        public async Task<Result<bool>> DeleteGroup(int id)
        {
            var group = await _unitOfWork.ClassGroups.GetByIdAsync(id);
            if (group == null)
            {
                return Result<bool>.Fail("Class group not found", 404);
            }
            _unitOfWork.ClassGroups.Delete(group);
            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, 200, "Class group deleted successfully");
        }

        public async Task<Result<bool>> DeleteGroups(List<int> ids)
        {
            var groups = await _unitOfWork.ClassGroups.GetByIdsAsync(ids);

            if (groups == null || !groups.Any())
            {
                return Result<bool>.Fail("No class groups found with the provided IDs", 404);
            }

            foreach (var group in groups)
            {
                _unitOfWork.ClassGroups.Delete(group);
            }

            await _unitOfWork.CommitAsync();
            return Result<bool>.Success(true, 200, "Class groups deleted successfully");
        }

        public async Task<Result<PagedResult<ClassGroupDto>>> GetAllGroups(string? search, int page, int pageSize)
        {
            var groups = await _unitOfWork.ClassGroups.GetAllWithRelatedDataAsync();
            if (!string.IsNullOrEmpty(search))
            {
                groups = groups.Where(g => g.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                           g.Course.Title.Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            var totalRecords = groups.Count();
            var pagedGroups = groups.Skip((page - 1) * pageSize).Take(pageSize);

            var dtos = _mapper.Map<List<ClassGroupDto>>(pagedGroups);

            var pagedResult = new PagedResult<ClassGroupDto>
            {
                Items = dtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };

            return Result<PagedResult<ClassGroupDto>>.Success(pagedResult, 200, "Class groups retrieved successfully");
        }
        public async Task<Result<List<ClassGroupDto>>> GetGroupsByCourseId(int courseId)
        {
            var groups = await _unitOfWork.ClassGroups.GetAllWithRelatedDataAsync();

            var courseGroups = groups
                .Where(g => g.CourseId == courseId)
                .ToList();

            if (!courseGroups.Any())
            {
                return Result<List<ClassGroupDto>>.Fail(
                    "No class groups found for the specified course ID",
                    404
                );
            }

            var dto = _mapper.Map<List<ClassGroupDto>>(courseGroups);

            return Result<List<ClassGroupDto>>.Success(
                dto,
                200,
                "Class groups retrieved successfully"
            );
        }


        public async Task<Result<ClassGroupDetailedDto>> GetGroupsByIdAsync(int id)
        {
            var group = await _unitOfWork.ClassGroups.GetByIdWithDetailsAsync(id);
            if (group == null)
            {
                return Result<ClassGroupDetailedDto>.Fail("Class group not found", 404);
            }

            var dto = _mapper.Map<ClassGroupDetailedDto>(group);

            return Result<ClassGroupDetailedDto>.Success(dto, 200, "Class group retrieved successfully");
        }

        public async Task<Result<ClassGroupDto>> UpdateGroup(int id, AddUpdateClassGroupDto group)
        {
            var existingGroup = await _unitOfWork.ClassGroups.GetByIdAsync(id);
            if (existingGroup == null)
                return Result<ClassGroupDto>.Fail("Class group not found", 404);
            _mapper.Map(group, existingGroup);
            _unitOfWork.ClassGroups.Update(existingGroup);
            await _unitOfWork.CommitAsync();
            var createdGroup = await _unitOfWork.ClassGroups.GetByIdAsync(existingGroup.GroupId);

            var classGroupDto = _mapper.Map<ClassGroupDto>(createdGroup);

            return Result<ClassGroupDto>.Success(classGroupDto, 201, "Class group Updated successfully");
        }

    }
}
