using System;

namespace IcampusBoatBackend.Models.Attendance
{
    public class BatchDeleteSectionRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? AcademicYear { get; set; }
    }

    public class BatchDeletePeriodLoadRequest
    {
        public string? Shift { get; set; } = "1";
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Stream { get; set; } = "1";
        public string? Section { get; set; }
        public string? Day { get; set; }
        public string? AcademicYear { get; set; }
    }

    public class BatchDeleteFacultyRequest
    {
        public string? Day { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Section { get; set; }
        public string? Semester { get; set; }
        public string? AcademicYear { get; set; }
        public string? Period { get; set; }
    }

    public class BatchDeleteStudentsRequest
    {
        public string? Day { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Section { get; set; }
        public string? Semester { get; set; }
        public string? Lecturer { get; set; }
        public string? AcademicYear { get; set; }
        public string? Period { get; set; }
    }

    public class BatchDeleteExecuteRequest
    {
        public string? Day { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Section { get; set; }
        public string? Semester { get; set; }
        public string? Lecturer { get; set; }
        public string? AcademicYear { get; set; }
        public string? UserId { get; set; }
        public string? Period { get; set; }
    }
}
