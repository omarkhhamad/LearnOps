namespace Domain.Models
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public decimal HourlyRate { get; set; }
        // Navigation Properties
        public ICollection<ClassGroup> ClassGroups { get; set; }= new List<ClassGroup>();

    }
}
