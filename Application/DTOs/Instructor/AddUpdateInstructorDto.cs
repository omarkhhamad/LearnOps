using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Instructor
{
    public class AddUpdateInstructorDto
    {
        /// <summary>Instructor full name</summary>
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = null!;

        /// <summary>Instructor phone number</summary>
        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; } = null!;

        /// <summary>Instructor email address</summary>
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        /// <summary>Hourly rate</summary>
        [Required(ErrorMessage = "Hourly rate is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Hourly rate must be positive")]
        public decimal HourlyRate { get; set; }
    }
}
