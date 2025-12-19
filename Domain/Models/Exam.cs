namespace Domain.Models
{
    public class Exam
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public int MaxScore { get; set; }

        // Foreign Keys
        public int GroupId { get; set; }

        // Navigation Properties
        public ClassGroup ClassGroup { get; set; } = null!;
        public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    }
}
