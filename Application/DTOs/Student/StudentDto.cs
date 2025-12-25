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
        /// <summary>Student full name</summary>
        /// <example>Mahmoud Taha</example>
        public string FullName { get; set; } = null!;
        /// <summary>Student phone number</summary>
        /// <example>01023140265</example>
        public string Phone { get; set; } = null!;

        /// <summary>Student email address</summary>
        /// <example>amer140106@email.com</example>
        public string? Email { get; set; }
        /// <summary>Date of birth</summary>
        /// <example>2002-07-21</example>
        public DateTime DateOfBirth { get; set; }
    }
}
