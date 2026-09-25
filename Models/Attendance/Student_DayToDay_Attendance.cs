using System;

namespace IcampusBoatBackend.Models.Attendance
{
    public class StudentDayToDayAttendanceRequest
    {
        public string? RegistrationNo { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? AcademicYear { get; set; }
    }
}
