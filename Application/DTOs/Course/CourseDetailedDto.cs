public class CourseDetailedDto
{
    public int CourseId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int DurationWeeks { get; set; }
    public decimal Price { get; set; }
    public int MaxStudents { get; set; }
    
    // Groups for this course
    public List<ClassGroupWithInstructorDto> Groups { get; set; }
    
    // Summary statistics
    public int TotalEnrolledStudents { get; set; }
    public int ActiveGroups { get; set; }
}

public class ClassGroupWithInstructorDto
{
    public int GroupId { get; set; }
    public string Name { get; set; }
    public string Room { get; set; }
    public string Days { get; set; }
    public string Time { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
        
    // Students in this group
    public int StudentsCount { get; set; }
}