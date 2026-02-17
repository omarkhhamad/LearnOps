using System;
using System.Collections.Generic;

namespace Application.DTOs.Profile
{
    public class UserProfileDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<string> Roles { get; set; } = new List<string>();

        // Student Specific
        public int? StudentId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Major { get; set; }

        // Instructor Specific
        public int? InstructorId { get; set; }
        public decimal? HourlyRate { get; set; }
        public string? Specialization { get; set; }
        public string? Degree { get; set; }
    }

    public class UpdateProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Potential additions for student/instructor
        public DateTime? DateOfBirth { get; set; }
        public string? Major { get; set; }
        public decimal? HourlyRate { get; set; }
        public string? Specialization { get; set; }
        public string? Degree { get; set; }
    }
}
