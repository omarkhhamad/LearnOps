using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ExamResult
    {

        public int ExamResultId { get; set; }
        public int Score { get; set; }
        public string? Result { get; set; }
        // Foreign Keys
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        // Navigation Properties
        public Student? Student { get; set; } = null!;
        public Exam Exam { get; set; } = null!;

    }
}
