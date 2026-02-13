namespace Domain.Models
{
    public class Enrollment:BaseEntity
    {
        public int EnrollmentId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = null!;
        // Foreign Keys
        public int GroupId { get; set; }
        public int StudentId { get; set; }

        // Navigation Properties
        public Student Student { get; set; } = null!;
        public Certificate? Certificate { get; set; }
        public ClassGroup ClassGroup { get; set; } = null!;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<ExamResult>? ExamResults { get; set; } = new List<ExamResult>();
    }
}
