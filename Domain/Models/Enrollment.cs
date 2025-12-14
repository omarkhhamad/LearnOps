namespace Domain.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Status { get; set; } = null!;
        // Foreign Keys
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        // Navigation Properties
        public Student Student { get; set; } = null!;
        public ClassGroup ClassGroup { get; set; } = null!;
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
        public Certificate Certificate { get; set; }
        public ICollection<ExamResult> ExamResults { get; set; }
    }
}
