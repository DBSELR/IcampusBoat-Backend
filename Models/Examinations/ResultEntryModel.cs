using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Examinations
{
    public class ResultEntryFilterModel
    {
        public string? AcademicYear { get; set; }
        public string? Department { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Stream { get; set; } = "1";
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
        public string? Lecturer { get; set; }
        public string? UserId { get; set; }
        public string? RegistrationNo { get; set; }
        public string? MINMARKS { get; set; }
        public string? MAXMARKS { get; set; }
        public string? SESSIONAL { get; set; }
    }

    public class ResultEntrySaveModel
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
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubMaxMrk { get; set; }
        public string? Sub_Max_MRK { get; set; }
        public string? MaxMrk { get; set; }
        public string? Max_MRK { get; set; }
        public string? Marks { get; set; }
        public string? Grade { get; set; }
        public string? SGPA { get; set; }
        public string? CGPA { get; set; }
        public string? AcademicYear { get; set; }
        public string? Lecturer { get; set; }
        public string? UserId { get; set; }
        public List<StudentResultItem>? Students { get; set; }
    }

    public class StudentResultItem
    {
        public string? Id { get; set; } = "0";
        public string? RegistrationNo { get; set; }
        public string? StudentName { get; set; }
        public string? Marks { get; set; }
        public string? Grade { get; set; }
        public string? SGPA { get; set; }
        public string? CGPA { get; set; }
        public string? Remarks { get; set; }
    }
}
