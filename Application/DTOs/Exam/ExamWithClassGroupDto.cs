using Application.DTOs.ClassGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Exam
{
    public class ExamWithClassGroupDto
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public int MaxScore { get; set; }
        public int GroupId { get; set; }
        public ClassGroupDto? ClassGroup { get; set; }
        public List<ClassGroupExamResult> ExamResults { get; set; } = new List<ClassGroupExamResult>();
    }
    public class ClassGroupExamResult
    {
        public string StudentName { get; set; } = string.Empty;
        public int Score { get; set; }
        public string? Result { get; set; }
    
    }
}
