using System;

namespace IcampusBoatBackend.Models.Attendance
{
    public class SectionFilterRequest
    {
        public string? AcdYr { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
    }

    public class LecturerFilterRequest
    {
        public string? Programme { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Subcode { get; set; }
        public string? Department { get; set; }
    }

    public class SubjectFilterRequest
    {
        public string? AcdYr { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Stream { get; set; }
        public string? PeriodType { get; set; }
        public string? Lecturer { get; set; }
        public string? Regu { get; set; }
        public string? Subtype { get; set; }
    }

    public class TimingsFilterRequest
    {
        public string? ShiftNo { get; set; }
        public string? Programme { get; set; }
        public string? Period { get; set; }
        public string? Toperiod { get; set; }
        public string? Year { get; set; }
    }

    public class TimeTableSearchRequest
    {
        public string? ShiftNo { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Section { get; set; }
        public string? Stream { get; set; }
        public string? AcdYr { get; set; }
    }

    public class CheckCountRequest
    {
        public string? ShiftNo { get; set; }
        public string? Day { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Section { get; set; }
        public string? Stream { get; set; }
        public string? Period { get; set; }
        public string? Subcode { get; set; }
        public string? Lecturer { get; set; }
        public string? AcademicYear { get; set; }
    }

    public class TimeTableSaveRequest
    {
        public string? Id { get; set; } = "0";
        public string? ShiftNo { get; set; }
        public string? Day { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Stream { get; set; }
        public string? Section { get; set; }
        public string? Period { get; set; }
        public string? Subject { get; set; }
        public string? Department { get; set; }
        public string? Lecturer { get; set; }
        public string? SPTime { get; set; }
        public string? PeriodType { get; set; }
        public string? Toperiod { get; set; }
        public string? Frrom_To_Periods { get; set; }
        public string? Subcode { get; set; }
        public string? EPTime { get; set; }
        public string? AcademicYear { get; set; }
        public string? LecStatus { get; set; }
        public string? EFRMDATE { get; set; }
        public string? Subtype { get; set; }
    }

    public class TimeTableDeleteRequest
    {
        public string? ShiftNo { get; set; }
        public string? Day { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Section { get; set; }
        public string? Stream { get; set; }
        public string? Frrom_To_Periods { get; set; }
        public string? Subcode { get; set; }
        public string? Proc_type { get; set; }
        public string? Wdate { get; set; }
    }

    public class SubTypeFilterRequest
    {
        public string? Subcode { get; set; }
        public string? EFRMDATE { get; set; }
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
    }

    public class TimeTableReportRequest
    {
        public string? Programme { get; set; }
        public string? Stream { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
    }
}

