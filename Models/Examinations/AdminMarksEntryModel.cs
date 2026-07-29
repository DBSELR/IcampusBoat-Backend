using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Examinations
{
    public class AdminMarksEntryFilterModel
    {
        public string? AcademicYear { get; set; }
        public string? Department { get; set; }
        public string? UserId { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? SubjectCode { get; set; }
        public string? MidType { get; set; }
        public string Stream { get; set; } = "1";
    }

    public class AdminMarksEntrySaveModel
    {
        public string? Id { get; set; } = "0";
        public string? RegistrationNo { get; set; }
        public string? Date { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Stream { get; set; } = "1";
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public string? MaxMarks { get; set; }
        public string? MinMarks { get; set; }
        public string? Marks { get; set; }
        public string? MidType { get; set; }
        public string? MidType1 { get; set; }
        public string? AcademicYear { get; set; }
        public string? UserId { get; set; }
    }

    public class AdminMarksEntryAttendanceSaveModel
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Stream { get; set; } = "1";
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public string? Date { get; set; }
        public string? UserId { get; set; }
        public List<AdminStudentAttendanceItem>? Students { get; set; }
    }

    public class AdminStudentAttendanceItem
    {
        public string? Id { get; set; } = "0";
        public string? RegistrationNo { get; set; }
        public string? Marks { get; set; }
        public string? TLMCode { get; set; }
        public string? TC { get; set; } = "0";
        public string? PC { get; set; } = "0";
        public string? Perc { get; set; } = "0";
    }
}
