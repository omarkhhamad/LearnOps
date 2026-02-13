using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Course;
using Application.DTOs.Instructor;
using Application.DTOs.Student;

namespace Application.DTOs.ClassGroup
{
    public class ClassGroupDetailedDto
    {
        public int GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string Room { get; set; } = null!;
        public string Days { get; set; } = null!;
        public string Time { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status => (EndDate == null || EndDate > DateTime.Now) ? "Active" : "Inactive";
        public CourseDto Course { get; set; } = null!;
        public InstructorDto Instructor { get; set; } = null!;
        public List<StudentDto> Students { get; set; } = new();
        public int StudentsCount => Students?.Count ?? 0;

    }
}
