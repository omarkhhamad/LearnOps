public class StudentDetailedDto
{
    public int StudentId { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public DateTime? DateOfBirth { get; set; }

    // Enrollments with full details
    public List<StudentCourseGroupDto> Courses { get; set; } = new();
}

public class StudentCourseGroupDto
{
    public int CourseId { get; set; }
    public string CourseName { get; set; } = null!;
    public int GroupId { get; set; }
    public string GroupName { get; set; } = null!;
}

