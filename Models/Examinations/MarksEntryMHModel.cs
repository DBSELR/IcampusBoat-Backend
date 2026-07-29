using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Examinations
{
    public class MarksEntryMHFilterModel
    {
        public string? AcademicYear { get; set; }
        public string? Department { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? SubjectCode { get; set; }
        public string? MidType { get; set; }
        public string? UserId { get; set; }
        public string Stream { get; set; } = "1";
    }

    public class MarksEntryMHSaveModel
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public string? MidType { get; set; }
        public string? Date { get; set; }
        public string? UserId { get; set; }
        public List<StudentMarksMHItem>? Students { get; set; }
        public string? Stream { get; set; }
    }

    public class StudentMarksMHItem
    {
        public string? Id { get; set; } = "0";
        public string? RegistrationNo { get; set; }
        public string? StudentName { get; set; }
        public string? HeadCode { get; set; }
        public string? HeadName { get; set; }
        public string? SubMaxMrk { get; set; }
        public string? MaxMarks { get; set; }
        public string? Marks { get; set; }
        public string? Grade { get; set; }
        public string? CGPA { get; set; }
        public string? SGPA { get; set; }
        public Dictionary<string, string>? HeadMarks { get; set; }
    }
}

