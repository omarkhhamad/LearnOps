
namespace Domain.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public ICollection<Certificate>Certificates { get; set; }= new List<Certificate>();
        public ICollection<Enrollment>Enrollments { get; set; }= new List<Enrollment>();

    }
}
