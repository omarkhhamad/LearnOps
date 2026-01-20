namespace Domain.Models
{
    //public enum AttendanceStatus
    //{
    //    Present,
    //    Absent,
    //    Late
    //}
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int EnrollmentId { get; set; }
        public DateTime SessionDate { get; set; }
        public string Status { get; set; } = null!;

        // Navigation Properties


    }
}
