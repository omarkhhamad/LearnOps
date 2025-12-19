using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Student
{
    public class StudentDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
