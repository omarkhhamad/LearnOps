namespace Domain.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        public DateTime IssuedDate { get; set; }
        // Foreign Keys
        public int EnrollmentId { get; set; }
        // Navigation Properties
        public Enrollment Enrollment { get; set; } = null!;

    }
}
