namespace Domain.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int EnrollmentId { get; set; }
        public DateTime SessionDate { get; set; }
        public string Status { get; set; } = null!;

        // Navigation Properties
        public Enrollment Enrollment { get; set; }= null!;


    }
}
