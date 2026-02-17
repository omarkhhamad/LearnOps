
namespace Domain.Models
{
    public class Student : BaseEntity
    {
        public int StudentId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Major { get; set; }

        // Navigation Properties
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();

    }
}
