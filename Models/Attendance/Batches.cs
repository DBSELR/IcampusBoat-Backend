using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Attendance
{
    public class BatchSectionSearchRequest
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Branch { get; set; }
    }

    public class BatchPeriodLoadRequest
    {
        public string? AcademicYear { get; set; }
        public string? Shift { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Stream { get; set; }
        public string? Section { get; set; }
        public string? Day { get; set; }
    }

    public class BatchSubjectsLoadRequest
    {
        public string? AcademicYear { get; set; }
        public string? Shift { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Stream { get; set; }
        public string? Section { get; set; }
        public string? Day { get; set; }
        public string? PeriodRange { get; set; }
    }

    public class BatchLecturerLoadRequest
    {
        public string? AcademicYear { get; set; }
        public string? Shift { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Stream { get; set; }
        public string? Section { get; set; }
        public string? Day { get; set; }
        public string? Subjects { get; set; }
        public string? PeriodRange { get; set; }
    }

    public class BatchStudentsLoadRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Stream { get; set; }
        public string? Day { get; set; }
        public string? PeriodRange { get; set; }
        public string? AcademicYear { get; set; }
        public string? Subjects { get; set; }
        public string? Lecturer { get; set; }
        public bool IsLecturerView { get; set; } = false;
    }

    public class BatchStudentItem
    {
        public string? RegNo { get; set; }
        public string? SName { get; set; }
        public bool Selected { get; set; }
    }

    public class SaveBatchStudentsRequest
    {
        public string? Lecturer { get; set; }
        public string? Shift { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Stream { get; set; }
        public string? Day { get; set; }
        public string? PeriodFrom { get; set; }
        public string? Subjects { get; set; }
        public string? PeriodRange { get; set; }
        public string? AcademicYear { get; set; }
        public List<BatchStudentItem>? Students { get; set; }
    }

    public class BatchTimeTableDataRequest
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? PeriodFrom { get; set; }
        public string? Day { get; set; }
    }

    public class EditEFromDateRequest
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? PeriodFrom { get; set; }
        public string? Day { get; set; }
        public string? NewEFromDate { get; set; }
        public string? UserId { get; set; }
    }
}
