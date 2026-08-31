using System;

namespace IcampusBoatBackend.Models.Attendance
{
    public class AdminSectionSearchRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? AcdYr { get; set; }
    }

    public class LecturerLoadRequest
    {
        public string? AcdYr { get; set; }
        public string? Lecturer { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Date { get; set; }
        public string? SubType { get; set; }
        public string? Shift { get; set; }
    }

    public class AdminPeriodLoadRequest
    {
        public string? AcdYr { get; set; }
        public string? Lecturer { get; set; }
        public string? Shift { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Date { get; set; }
    }

    public class AdminLoadStudentsRequest
    {
        public string? Lecturer { get; set; }
        public string? Period { get; set; }
        public string? Date { get; set; }
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public bool IsPractical { get; set; } = false;
        public string? SrNo { get; set; }
        public string? ErNo { get; set; }
    }

    public class SaveAdminAttendanceSubWiseRequest
    {
        public string? Lecturer { get; set; }
        public string? Semester { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Section { get; set; }
        public string? Period { get; set; }
        public string? Subjects { get; set; }
        public string? AcademicYear { get; set; }
        public string? Day { get; set; }
        public string? Date { get; set; }
        public string? SrNo { get; set; }
        public string? ErNo { get; set; }
        public string? DayTaught { get; set; }
        public string? AttStat { get; set; }
        public string? Query { get; set; }
        public string? PeriodRange { get; set; }
        public string? TLM { get; set; }
    }

    public class SaveAdminPermissionsRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Section { get; set; }
        public string? Semester { get; set; }
        public string? Lecturer { get; set; }
        public string? Subject { get; set; }
        public string? Date { get; set; }
        public string? AcdYr { get; set; }
    }
}

