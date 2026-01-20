using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ExamResult
    {
    
        public int ExamResultId { get; set; }
        public int Score { get; set; }
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public string? Result { get; set; }
        // Navigation Properties
        public Student? Student { get; set; } = null!;
        public Exam Exam { get; set; } = null!;

        // Optional foreign key to Enrollment
        //public int? EnrollmentId { get; set; }
        //public Enrollment? Enrollment { get; set; }

    }
}
