namespace Domain.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int EnrollmentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Method { get; set; } = null!;

        // Navigation Properties
        public Enrollment Enrollment { get; set; }= null!;
    }
}
