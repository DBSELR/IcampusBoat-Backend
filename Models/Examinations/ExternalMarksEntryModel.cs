using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Examinations
{
    public class ExternalSubjectFilterModel
    {
        public string? UserId { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? AcademicYear { get; set; }
        public string Stream { get; set; } = "1";
    }

    public class ExternalMidTypeFilterModel
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? SubjectCode { get; set; }
        public string? Section { get; set; }
        public string? AcademicYear { get; set; }
    }

    public class ExternalMaxMinMarksFilterModel
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? SubjectCode { get; set; }
        public string? MidType { get; set; }
    }

    public class ExternalInternalDateFilterModel
    {
        public string? Programme { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? AcademicYear { get; set; }
        public string? MidType { get; set; }
    }

    public class ExternalStudentMarksFilterModel
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Year { get; set; }
        public string? SubjectCode { get; set; }
        public string? UserId { get; set; }
        public string? MidType { get; set; }
    }

    public class ExternalValidateRegNoFilterModel
    {
        public string? RegistrationNo { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? Semester { get; set; }
        public string? SubjectCode { get; set; }
        public string? Section { get; set; }
        public string? Sessional { get; set; }
    }

    public class ExternalMarksEntrySaveModel
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
        public string? MaxMarks { get; set; }
        public string? Marks { get; set; }
        public string? MidType { get; set; }
        public string? AcademicYear { get; set; }
        public string? UserId { get; set; }
    }

    public class ExternalMarksEntryFreezeModel
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Year { get; set; }
        public string? SubjectCode { get; set; }
        public string? Lecturer { get; set; }
        public string? MidType { get; set; }
    }
}
