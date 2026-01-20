using Application.DTOs.Exam;
using Application.Interfaces.IServices;
using Application.Result;
using Application.UnitOfWork;
using AutoMapper;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ExamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<ExamDto>> CreateExam(ExamDto examDto)
        {
            // Validate input
            if (examDto == null)
            {
                return Result<ExamDto>.Fail("Exam data cannot be null", 400);
            }

            if (examDto.GroupId <= 0)
            {
                return Result<ExamDto>.Fail("Invalid Group ID", 400);
            }

            if (string.IsNullOrWhiteSpace(examDto.Title))
            {
                return Result<ExamDto>.Fail("Exam title is required", 400);
            }

            if (examDto.MaxScore <= 0)
            {
                return Result<ExamDto>.Fail("Max score must be greater than zero", 400);
            }

            // Verify that the group exists
            var groupExists = await _unitOfWork.ClassGroups.GetByIdAsync(examDto.GroupId);
            if (groupExists == null)
            {
                return Result<ExamDto>.Fail($"Class group with ID {examDto.GroupId} not found", 404);
            }

            // Check for duplicate exam in the same group
            var isDuplicate = await _unitOfWork.Exams.ExistInGroupAsync(examDto.GroupId, examDto.Title);
            if (isDuplicate)
            {
                return Result<ExamDto>.Fail($"An exam with title '{examDto.Title}' already exists in this group", 409);
            }

            try
            {
                var newExam = _mapper.Map<Exam>(examDto);
                await _unitOfWork.Exams.AddAsync(newExam);
                await _unitOfWork.CommitAsync();

                var createdDto = _mapper.Map<ExamDto>(newExam);
                return Result<ExamDto>.Success(createdDto, 201, "Exam created successfully");
            }
            catch (System.Exception ex)
            {
                return Result<ExamDto>.Fail($"Error creating exam: {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> DeleteExam(int id)
        {
            if (id <= 0)
            {
                return Result<bool>.Fail("Invalid exam ID", 400);
            }

            var exam = await _unitOfWork.Exams.GetByIdAsync(id);
            if (exam == null)
            {
                return Result<bool>.Fail("Exam not found", 404);
            }

            try
            {
                _unitOfWork.Exams.Delete(exam);
                await _unitOfWork.CommitAsync();
                return Result<bool>.Success(true, 200, "Exam deleted successfully");
            }
            catch (System.Exception ex)
            {
                return Result<bool>.Fail($"Error deleting exam: {ex.Message}", 500);
            }
        }

        public async Task<Result<bool>> DeleteRangeOfExams(int [] examsIds)
        {
            try
                {
                var examsToDelete = new List<Exam>();
                foreach (var id in examsIds)
                {
                    //examsToDelete.Add(new Exam { ExamId = id });
                    var exam = await _unitOfWork.Exams.GetByIdAsync(id);
                    if (exam != null)
                    {
                        examsToDelete.Add(exam);
                    }
                }
                await _unitOfWork.Exams.DeleteRange(examsToDelete);
                await _unitOfWork.CommitAsync();
                return Result<bool>.Success(true, 200, "Exams deleted successfully");
            }
            catch (System.Exception ex)
            {
                return Result<bool>.Fail($"Error deleting exams: {ex.Message}", 500);
            }
        }

        public async Task<Result<PagedResult<ExamWithClassGroupDto>>> GetAllExams(string? Search,int Page = 1,int PageSize =10)
        {
            try
            {
                var exams = await _unitOfWork.Exams.GetAllExamsAsync();
                
                var examDtos = _mapper.Map<List<ExamWithClassGroupDto>>(exams.ToList());
                if (!string.IsNullOrWhiteSpace(Search))
                {
                    examDtos = examDtos
                        .Where(e => e.Title != null && e.Title.ToLower().Contains(Search.ToLower()))
                        .ToList();
                }
                var TotalRecords = examDtos.Count;
                var taotalPages = (int)Math.Ceiling((double)TotalRecords / (int)PageSize);
                var PagedExams = examDtos.Skip(((int)Page - 1) * (int)PageSize).Take((int)PageSize).ToList();
                var pagedResult = new PagedResult<ExamWithClassGroupDto>
                {
                    Items = PagedExams,
                    CurrentPage = Page,
                    PageSize = PageSize,
                    TotalRecords = TotalRecords
                };

                return Result<PagedResult<ExamWithClassGroupDto>>.Success(
                    pagedResult,
                    200,
                    examDtos.Count > 0 ? $"Retrieved {examDtos.Count} exam(s)" : "No exams found"
                );
            }
            catch (System.Exception ex)
            {
                return Result<PagedResult<ExamWithClassGroupDto>>.Fail($"Error retrieving exams: {ex.Message}", 500);
            }
        }

        public async Task<Result<ExamWithClassGroupDto?>> GetExamById(int id)
        {
            if (id <= 0)
            {
                return Result<ExamWithClassGroupDto?>.Fail("Invalid exam ID", 400);
            }

            try
            {
                var exam = await _unitOfWork.Exams.GetExamByIdAsync(id);
                if (exam == null)
                {
                    return Result<ExamWithClassGroupDto?>.Fail("Exam not found", 404);
                }

                var examDto = _mapper.Map<ExamWithClassGroupDto>(exam);
                return Result<ExamWithClassGroupDto?>.Success(examDto, 200, "Exam retrieved successfully");
            }
            catch (System.Exception ex)
            {
                return Result<ExamWithClassGroupDto?>.Fail($"Error retrieving exam: {ex.Message}", 500);
            }
        }

        public async Task<Result<List<ExamWithClassGroupDto>>> GetExamsByCourseIdAsync(int courseId)
        {
            if (courseId <= 0)
            {
                return Result<List<ExamWithClassGroupDto>>.Fail("Invalid course ID", 400);
            }

            try
            {
                var exams = await _unitOfWork.Exams.GetExamsByCourseIdAsync(courseId);
                var examDtos = _mapper.Map<List<ExamWithClassGroupDto>>(exams.ToList());

                return Result<List<ExamWithClassGroupDto>>.Success(
                    examDtos,
                    200,
                    examDtos.Count > 0 ? $"Retrieved {examDtos.Count} exam(s) for course" : "No exams found for this course"
                );
            }
            catch (System.Exception ex)
            {
                return Result<List<ExamWithClassGroupDto>>.Fail($"Error retrieving exams by course: {ex.Message}", 500);
            }
        }

        public async Task<Result<List<ExamWithClassGroupDto>>> GetExamsByGroupIdAsync(int groupId)
        {
            if (groupId <= 0)
            {
                return Result<List<ExamWithClassGroupDto>>.Fail("Invalid group ID", 400);
            }

            try
            {
                var exams = await _unitOfWork.Exams.GetExamsByGroupIdAsync(groupId);
                var examDtos = _mapper.Map<List<ExamWithClassGroupDto>>(exams.ToList());

                return Result<List<ExamWithClassGroupDto>>.Success(
                    examDtos,
                    200,
                    examDtos.Count > 0 ? $"Retrieved {examDtos.Count} exam(s) for group" : "No exams found for this group"
                );
            }
            catch (System.Exception ex)
            {
                return Result<List<ExamWithClassGroupDto>>.Fail($"Error retrieving exams by group: {ex.Message}", 500);
            }
        }

        public async Task<Result<ExamWithClassGroupDto?>> GetExamWithResultsAsync(int examId)
        {
            if (examId <= 0)
            {
                return Result<ExamWithClassGroupDto?>.Fail("Invalid exam ID", 400);
            }

            try
            {
                var exam = await _unitOfWork.Exams.GetExamWithResultsAsync(examId);
                if (exam == null)
                {
                    return Result<ExamWithClassGroupDto?>.Fail("Exam not found", 404);
                }

                var examDto = _mapper.Map<ExamWithClassGroupDto>(exam);
                return Result<ExamWithClassGroupDto?>.Success(
                    examDto,
                    200,
                    $"Exam retrieved with {exam.ExamResults?.Count ?? 0} result(s)"
                );
            }
            catch (System.Exception ex)
            {
                return Result<ExamWithClassGroupDto?>.Fail($"Error retrieving exam with results: {ex.Message}", 500);
            }
        }

        public async Task<Result<ExamDto>> UpdateExam(ExamDto examDto, int id)
        {
            if (id <= 0)
            {
                return Result<ExamDto>.Fail("Invalid exam ID", 400);
            }

            if (examDto == null)
            {
                return Result<ExamDto>.Fail("Exam data cannot be null", 400);
            }

            if (string.IsNullOrWhiteSpace(examDto.Title))
            {
                return Result<ExamDto>.Fail("Exam title is required", 400);
            }

            if (examDto.MaxScore <= 0)
            {
                return Result<ExamDto>.Fail("Max score must be greater than zero", 400);
            }

            try
            {
                var existingExam = await _unitOfWork.Exams.GetByIdAsync(id);
                if (existingExam == null)
                {
                    return Result<ExamDto>.Fail("Exam not found", 404);
                }

                // Check if updating to a title that already exists in the group (excluding current exam)
                if (existingExam.GroupId == examDto.GroupId && existingExam.Title != examDto.Title)
                {
                    var isDuplicate = await _unitOfWork.Exams.ExistInGroupAsync(examDto.GroupId, examDto.Title);
                    if (isDuplicate)
                    {
                        return Result<ExamDto>.Fail($"An exam with title '{examDto.Title}' already exists in this group", 409);
                    }
                }

                // If GroupId is being changed, verify the new group exists
                if (existingExam.GroupId != examDto.GroupId)
                {
                    var newGroupExists = await _unitOfWork.ClassGroups.GetByIdAsync(examDto.GroupId);
                    if (newGroupExists == null)
                    {
                        return Result<ExamDto>.Fail($"Class group with ID {examDto.GroupId} not found", 404);
                    }
                }

                _mapper.Map(examDto, existingExam);
                _unitOfWork.Exams.Update(existingExam);
                await _unitOfWork.CommitAsync();

                return Result<ExamDto>.Success(examDto, 200, "Exam updated successfully");
            }
            catch (System.Exception ex)
            {
                return Result<ExamDto>.Fail($"Error updating exam: {ex.Message}", 500);
            }
        }


    }
}