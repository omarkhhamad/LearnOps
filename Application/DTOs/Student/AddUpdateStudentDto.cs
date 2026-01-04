using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Student
{
    public class AddUpdateStudentDto
    {
        /// <summary>Student full name</summary>
        /// <example>Mahmoud Taha</example>
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string FullName { get; set; } = null!;

        /// <summary>Student phone number</summary>
        /// <example>01023140265</example>
        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string Phone { get; set; } = null!;

        /// <summary>Student email address</summary>
        /// <example>amer140106@email.com</example>
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        /// <summary>Date of birth</summary>
        /// <example>2002-07-21</example>
        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
    }
}
