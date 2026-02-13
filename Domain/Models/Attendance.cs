namespace Domain.Models
{
    public enum AttendanceStatus
    {
        Present,
        Absent,
        Late
    }
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int EnrollmentId { get; set; }
        public DateTime SessionDate { get; set; }
        public AttendanceStatus Status { get; set; }

        public Enrollment Enrollment { get; set; } = null!;

    }
}
