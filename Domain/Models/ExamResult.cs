using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ExamResult
    {
        [Key]
        public int ResultId { get; set; }
        public int ExamId { get; set; }
        public int Score { get; set; }
        public int EnrollmentId { get; set; }
        public string Result { get; set; } = null!;
        // Navigation Properties
        public Enrollment Enrollment { get; set; } = null!;
        public Exam Exam { get; set; } = null!;

    }
}
