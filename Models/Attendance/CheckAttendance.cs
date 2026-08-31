using System;

namespace IcampusBoatBackend.Models.Attendance
{
    public class CheckAttendanceSectionRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? AcdYr { get; set; }
    }

    public class CheckAttendancePeriodLoadRequest
    {
        public string? AcdYr { get; set; }
        public string? Lecturer { get; set; }
        public string? Date { get; set; }
        public string? Shift { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? SYear { get; set; }
    }

    public class CheckDateValidityRequest
    {
        public string? Date { get; set; }
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
    }

    public class CheckAttendanceStudentsRequest
    {
        public string? Period { get; set; }
        public string? Lecturer { get; set; }
        public string? Date { get; set; }
        public string? AcademicYear { get; set; }
        public string? Section { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Programme { get; set; }
        public string? Semester { get; set; }
        public bool IsPractical { get; set; } = false;
        public string? SrNo { get; set; }
        public string? ErNo { get; set; }
    }
}
