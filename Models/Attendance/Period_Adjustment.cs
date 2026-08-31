using System;

namespace IcampusBoatBackend.Models.Attendance
{
    public class FacultyFilterRequest
    {
        public string? EmpId { get; set; }
        public string? Dept { get; set; }
        public string? WorkMode { get; set; } = "Teaching";
        public string? Access { get; set; }
    }

    public class AbsFacTimeTableRequest
    {
        public string? Lecturer { get; set; }
        public string? Day { get; set; }
        public string? Wdate { get; set; }
        public string? Programme { get; set; }
        public string? Semister { get; set; }
        public string? AcdYr { get; set; }
    }

    public class AvailableFacultyRequest
    {
        public string? Fac { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Branch { get; set; }
        public string? Section { get; set; }
        public string? Day { get; set; }
        public string? PeriodFrom { get; set; }
        public string? PeriodTo { get; set; }
        public string? Programme { get; set; }
        public string? Wdate { get; set; }
        public string? Lecturer { get; set; }
        public string? AcdYr { get; set; }
    }

    public class AvailableFacultySubRequest
    {
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Branch { get; set; }
        public string? Section { get; set; }
        public string? Day { get; set; }
        public string? PeriodFrom { get; set; }
        public string? PeriodTo { get; set; }
        public string? Programme { get; set; }
        public string? Wdate { get; set; }
        public string? Lecturer { get; set; }
        public string? AcdYr { get; set; }
    }

    public class TimeTableAdjustSaveRequest
    {
        public string? ShiftNo { get; set; }
        public string? Day { get; set; }
        public string? Wdate { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Section { get; set; }
        public string? Period { get; set; }
        public string? Subject { get; set; }
        public string? AVL_Subject { get; set; }
        public string? Department { get; set; }
        public string? Lecturer { get; set; }
        public string? AVL_Lecturert { get; set; }
        public string? Stream { get; set; }
        public string? SPTime { get; set; }
        public string? PeriodType { get; set; }
        public string? Frrom_To_Periods { get; set; }
        public string? Toperiod { get; set; }
        public string? Reason { get; set; }
        public string? P_SubjectCode { get; set; }
        public string? L_SubjectCode { get; set; }
    }

    public class TimeTableDeleteAdjustmentRequest
    {
        public string? ShiftNo { get; set; }
        public string? Day { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Section { get; set; }
        public string? Stream { get; set; }
        public string? Frrom_To_Periods { get; set; }
        public string? Proc_type { get; set; } = "TT_ADJUST";
        public string? Wdate { get; set; }
    }

    public class PeriodCancelSaveRequest
    {
        public string? ShiftNo { get; set; }
        public string? Day { get; set; }
        public string? Wdate { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Stream { get; set; }
        public string? Section { get; set; }
        public string? Period { get; set; }
        public string? Subject { get; set; }
        public string? Lecturer { get; set; }
        public string? PeriodType { get; set; }
        public string? Frrom_To_Periods { get; set; }
        public string? SPTime { get; set; }
        public string? Toperiod { get; set; }
        public string? Reason { get; set; }
    }

    public class PeriodCancelDeleteRequest
    {
        public string? ShiftNo { get; set; }
        public string? Wdate { get; set; }
        public string? Period { get; set; }
        public string? Toperiod { get; set; }
        public string? Branch { get; set; }
        public string? Section { get; set; }
        public string? Lecturer { get; set; }
    }
}

