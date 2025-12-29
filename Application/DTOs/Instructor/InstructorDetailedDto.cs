public class InstructorDetailedDto
{
    public int InstructorId { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public decimal HourlyRate { get; set; }

    // Courses taught
    public List<CourseWithGroupsDto> Courses { get; set; } = new();

    // Summary statistics
    public int TotalStudents { get; set; }
    public int ActiveGroups { get; set; }
}

public class CourseWithGroupsDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public List<GroupInfoDto> Groups { get; set; } = new();
}

public class GroupInfoDto
{
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
    public string Room { get; set; } = null!;
    public string Days { get; set; } = null!;
    public string Time { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int StudentsCount { get; set; }
    public string Status => (EndDate == null || EndDate > DateTime.Now) ? "Active" : "Inactive";
}
