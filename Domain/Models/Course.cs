namespace Domain.Models
{
    public class Course: BaseEntity
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int DurationWeeks { get; set; }
        public decimal Price { get; set; }
        public int MaxStudents { get; set; }

        // Navigation Properties
        public ICollection<ClassGroup> ClassGroups { get; set; }= new List<ClassGroup>();
    }
}
