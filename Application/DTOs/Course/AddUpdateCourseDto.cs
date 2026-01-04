using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Course
{
    public class AddUpdateCourseDto
    {
        /// <summary>Course title</summary>
        /// <example>Introduction to .NET</example>
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = null!;

        /// <summary>Course description</summary>
        /// <example>This course covers the basics of .NET and C#</example>
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = null!;

        /// <summary>Course duration in weeks</summary>
        /// <example>8</example>
        [Range(1, 52, ErrorMessage = "Duration must be between 1 and 52 weeks")]
        public int DurationWeeks { get; set; }

        /// <summary>Course price</summary>
        /// <example>2500</example>
        [Range(0, 10000, ErrorMessage = "Price must be between 0 and 1,000,000")]
        public decimal Price { get; set; }

        /// <summary>Maximum number of students allowed</summary>
        /// <example>30</example>
        [Range(1, 100, ErrorMessage = "MaxStudents must be at least 1")]
        public int MaxStudents { get; set; }
    }
}
