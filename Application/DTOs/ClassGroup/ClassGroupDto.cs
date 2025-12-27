using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DateTime EndDate { get; set; }
        public int CourseId { get; set; }
        public int InstructorId { get; set; }
    }
}
