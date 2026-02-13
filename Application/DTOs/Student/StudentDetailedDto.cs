using System;
using System.Collections.Generic;

namespace Application.DTOs.Student
{
    public class StudentDetailedDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public int Age => DateOfBirth.HasValue ? DateTime.Now.Year - DateOfBirth.Value.Year : 0;

        // Statistics
        public int TotalCourses { get; set; }
        public int ActiveCourses { get; set; }

        // Detailed Enrollments
        public List<StudentCourseDetailsDto> Enrollments { get; set; } = new();
    }

    public class StudentCourseDetailsDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int GroupId { get; set; }
        public string GroupName { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
