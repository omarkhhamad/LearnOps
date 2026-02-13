public class CourseDetailedDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DurationWeeks { get; set; }
    public decimal Price { get; set; }
    public int MaxStudents { get; set; }

    // Groups for this course
    public List<ClassGroupWithInstructorDto> Groups { get; set; } = new List<ClassGroupWithInstructorDto>();

    // Summary statistics
    public int TotalEnrolledStudents { get; set; }
    public int ActiveGroups { get; set; }
}

public class ClassGroupWithInstructorDto
{
    public int GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
    public string Days { get; set; } = string.Empty;
    public string Time { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Students in this group
    public int StudentsCount { get; set; }
}