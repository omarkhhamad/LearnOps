using System;
using System.Collections.Generic;
using Application.DTOs.Course;
using Application.DTOs.Instructor;
using Application.DTOs.Student;

namespace Application.DTOs.ClassGroup
{
    public class ClassGroupDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string Room { get; set; } = null!;
        public string Days { get; set; } = null!;
        public string Time { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string CourseName { get; set; } = null!;
        public string InstructorName { get; set; } = null!;
        public int StudentsCount { get; set; }

        public string Status => (EndDate == null || EndDate > DateTime.Now) ? "Active" : "Inactive";
    }

   
}