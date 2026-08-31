using System;

namespace IcampusBoatBackend.Models.Attendance
{
    public class ExtraSectionSearchRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? AcdYr { get; set; }
    }

    public class ExtraLecturerSearchRequest
    {
        public string? Department { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Subcode { get; set; }
        public string? Programme { get; set; }
    }

    public class ExtraSubjectLoadRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Stream { get; set; }
        public string? PeriodType { get; set; }
        public string? Lecturer { get; set; }
        public string? Regu { get; set; }
        public string? Subtype { get; set; }
        public string? AcdYr { get; set; }
    }

    public class ExtraTimingsRequest
    {
        public string? ShiftNo { get; set; }
        public string? Programme { get; set; }
        public string? Period { get; set; }
        public string? Toperiod { get; set; }
        public string? Year { get; set; }
    }

    public class TimeTableExtraViewRequest
    {
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Branch { get; set; }
        public string? Stream { get; set; }
        public string? Section { get; set; }
        public string? ShiftNo { get; set; }
        public string? Programme { get; set; }
    }

    public class TimeTableExtraSaveRequest
    {
        public string? Id { get; set; } = "0";
        public string? Wdate { get; set; }
        public string? ShiftNo { get; set; }
        public string? Day { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Stream { get; set; }
        public string? Section { get; set; }
        public string? Subject { get; set; }
        public string? Subcode { get; set; }
        public string? Department { get; set; }
        public string? Lecturer { get; set; }
        public string? SPTime { get; set; }
        public string? PeriodType { get; set; }
        public string? EPTime { get; set; }
    }
}
