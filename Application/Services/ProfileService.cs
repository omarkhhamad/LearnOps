using Application.Bases;
using Application.DTOs.Profile;
using Application.Interfaces.IServices;
using Application.UnitOfWork;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Result<UserProfileDto>> GetProfileAsync(Guid userId)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return Result<UserProfileDto>.Fail("User not found", 404);
                }

                var roles = await _userManager.GetRolesAsync(user);

                var profile = new UserProfileDto
                {
                    UserId = user.Id,
                    FullName = user.FullName,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    Bio = user.Bio,
                    LastLoginAt = user.LastLoginAt,
                    Roles = roles.ToList()
                };

                if (roles.Contains("Student"))
                {
                    var student = await _unitOfWork.Students.GetByUserIdAsync(userId);
                    if (student != null)
                    {
                        profile.StudentId = student.StudentId;
                        profile.DateOfBirth = student.DateOfBirth;
                        profile.Major = student.Major;
                    }
                }

                if (roles.Contains("Instructor"))
                {
                    var instructor = await _unitOfWork.Instructors.GetByUserIdAsync(userId);
                    if (instructor != null)
                    {
                        profile.InstructorId = instructor.InstructorId;
                        profile.HourlyRate = instructor.HourlyRate;
                        profile.Specialization = instructor.Specialization;
                        profile.Degree = instructor.Degree;
                    }
                }

                return Result<UserProfileDto>.Success(profile);
            }
            catch (Exception ex)
            {
                return Result<UserProfileDto>.Fail($"Error retrieving profile: {ex.Message}", 500);
            }
        }

        public async Task<Result> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return Result.Fail("User not found", 404);
                }

                user.FullName = request.FullName;
                user.PhoneNumber = request.PhoneNumber;
                user.Bio = request.Bio;
                user.ProfilePictureUrl = request.ProfilePictureUrl;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return Result.Fail(string.Join(", ", result.Errors.Select(e => e.Description)), 400);
                }

                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Student") && (request.DateOfBirth.HasValue || request.Major != null))
                {
                    var student = await _unitOfWork.Students.GetByUserIdAsync(userId);
                    if (student != null)
                    {
                        if (request.DateOfBirth.HasValue) student.DateOfBirth = request.DateOfBirth.Value;
                        if (request.Major != null) student.Major = request.Major;
                        _unitOfWork.Students.Update(student);
                    }
                }

                if (roles.Contains("Instructor") && (request.HourlyRate.HasValue || request.Specialization != null || request.Degree != null))
                {
                    var instructor = await _unitOfWork.Instructors.GetByUserIdAsync(userId);
                    if (instructor != null)
                    {
                        if (request.HourlyRate.HasValue) instructor.HourlyRate = request.HourlyRate.Value;
                        if (request.Specialization != null) instructor.Specialization = request.Specialization;
                        if (request.Degree != null) instructor.Degree = request.Degree;
                        _unitOfWork.Instructors.Update(instructor);
                    }
                }

                await _unitOfWork.CommitAsync();
                return Result.Success(200, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return Result.Fail($"Error updating profile: {ex.Message}", 500);
            }
        }
    }
}
