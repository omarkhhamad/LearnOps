using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Instructor
{
    public class InstructorDto
    {
        /// <summary>Instructor ID</summary>
        public int InstructorId { get; set; }

        /// <summary>Instructor full name</summary>
        public string FullName { get; set; } = null!;

        /// <summary>Instructor phone number</summary>
        public string Phone { get; set; } = null!;

        /// <summary>Instructor email address</summary>
        public string? Email { get; set; }

        /// <summary>Hourly rate</summary>
        public decimal HourlyRate { get; set; }
    }
}
