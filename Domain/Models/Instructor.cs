namespace Domain.Models
{
    public class Instructor : BaseEntity
    {
        public int InstructorId { get; set; }
        public decimal HourlyRate { get; set; }
        public string? Specialization { get; set; }
        public string? Degree { get; set; }

        // Navigation Properties
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
        public ICollection<ClassGroup> ClassGroups { get; set; } = new List<ClassGroup>();

    }
}
