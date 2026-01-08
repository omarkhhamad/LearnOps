using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class ClassGroup: BaseEntity
    {
        [Key]
        public int GroupId { get; set; }
        public string Name { get; set; } = null!;
        public string Room { get; set; } = null!;

        public string Days { get; set; } = null!;

        public string Time { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        // Foreign Keys
        public int CourseId { get; set; }
        public int InstructorId { get; set; }
        // Navigation Properties
        public Course Course { get; set; } = null!;
        public Instructor Instructor { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; set; }=new List<Enrollment>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();


    }
}
