using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Exam
{
    public class ExamDto
    {
            public string Title { get; set; } = string.Empty;
            public DateTime ExamDate { get; set; }
            public int MaxScore { get; set; }
            public int GroupId { get; set; }
    }
}
