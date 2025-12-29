using System;
using System.Text.Json.Serialization;
using Application.DTOs.ClassGroup;
using Application.DTOs.Instructor;

namespace Application.DTOs.Course
{
    public class CourseDto
    {
        /// <summary>Course ID</summary>
        /// <example>1</example>
        public int CourseId { get; set; }

        /// <summary>Course title</summary>
        /// <example>Introduction to .NET</example>
        public string Title { get; set; } = null!;

        /// <summary>Course description</summary>
        /// <example>This course covers the basics of .NET and C#</example>
        public string Description { get; set; } = null!;

        /// <summary>Course duration in weeks</summary>
        /// <example>8</example>
        public int DurationWeeks { get; set; }

        /// <summary>Course price</summary>
        /// <example>2500</example>
        public decimal Price { get; set; }

        /// <summary>Maximum number of students allowed</summary>
        /// <example>30</example>
        public int MaxStudents { get; set; }

    }
}
