using System;

namespace IcampusBoatBackend.Models.Attendance
{
    public class SectionSearchRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
    }

    public class PeriodLoadRequest
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

    public class MidAttDatesRequest
    {
        public string? AcdYr { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Period { get; set; }
        public string? Date { get; set; }
    }

    public class LoadStudentsRequest
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

    public class SaveAttendanceSubWiseRequest
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
        public string? UpdateDate { get; set; } = "NULL";
        public string? SrNo { get; set; }
        public string? ErNo { get; set; }
        public string? DayTaught { get; set; }
        public string? AttStat { get; set; }
        public string? Query { get; set; }
        public string? PeriodRange { get; set; }
        public string? TLM { get; set; }
    }

    public class AdminDatesCheckRequest
    {
        public string? Lecturer { get; set; }
        public string? Branch { get; set; }
        public string? AcdYr { get; set; }
        public string? Date { get; set; }
    }

    public class EmpPeriodsAttRequest
    {
        public string? Date { get; set; }
        public string? Day { get; set; }
        public string? Section { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Lecturer { get; set; }
    }
}

