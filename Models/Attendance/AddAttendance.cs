using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Attendance
{
    public class AddAttSectionRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? AcdYr { get; set; }
    }

    public class LoadAddAttRequest
    {
        public string? Semister { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Section { get; set; }
        public string? AcademicYear { get; set; }
        public string? FPerc { get; set; }
        public string? TPerc { get; set; }
    }

    public class LoadStudentAbsSubjectsRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Section { get; set; }
        public string? Regno { get; set; }
        public string? Subcode { get; set; } = "";
    }

    public class LoadStudentAbsDataRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Section { get; set; }
        public string? Regno { get; set; }
        public string? Subcode { get; set; }
        public string? AcademicYear { get; set; }
    }

    public class AbsenceItem
    {
        public string? Date { get; set; }
        public string? Faculty { get; set; }
        public string? Subcode { get; set; }
        public string? Period { get; set; }
        public bool Selected { get; set; }
    }

    public class SaveAddAttendanceRequest
    {
        public string? Id { get; set; } = "";
        public string? Regno { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semister { get; set; }
        public string? Section { get; set; }
        public string? AcademicYear { get; set; }
        public string? UserId { get; set; }
        public string? FPerc { get; set; }
        public string? TPerc { get; set; }
        public List<AbsenceItem>? AbsenceItems { get; set; }
    }

    public class SendAddAttOtpRequest
    {
        public string? UserId { get; set; }
        public string? SubGroup { get; set; }
        public string? UserMobile { get; set; }
        public string? UserEmail { get; set; }
    }
}
