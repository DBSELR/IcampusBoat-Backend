using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Examinations
{
    public class AttendanceMarksFilterModel
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? SubjectCode { get; set; }
        public string? UserId { get; set; }
    }

    public class AttendanceMarksSaveModel
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? SubjectCode { get; set; }
        public string? Date { get; set; }
        public string? UserId { get; set; }
        public List<StudentAttendanceMarkItem>? Students { get; set; }
    }

    public class StudentAttendanceMarkItem
    {
        public string? Id { get; set; } = "0";
        public string? RegistrationNo { get; set; }
        public string? StudentName { get; set; }
        public string? Marks { get; set; }
        public string? TLMCode { get; set; }
        public string? TotalClasses { get; set; } = "0";
        public string? PresentClasses { get; set; } = "0";
        public string? Percentage { get; set; } = "0";
    }
}
