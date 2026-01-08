namespace Domain.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        public DateTime IssuedDate { get; set; }
        // Foreign Keys
        public int EnrollmentId { get; set; }
        public int? StudentId { get; set; }
        // Navigation Properties
        public Enrollment Enrollment { get; set; } = null!;
        public Student? Student { get; set; }

    }
}
