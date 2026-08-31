using System;

namespace IcampusBoatBackend.Models.Attendance
{
    public class EditEFRMDateSectionRequest
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Branch { get; set; }
    }

    public class EditEFRMDatePeriodLoadRequest
    {
        public string? Shift { get; set; } = "1";
        public string? Stream { get; set; } = "1";
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Day { get; set; }
    }

    public class EditEFRMDateTimeTableDataRequest
    {
        public string? Shift { get; set; } = "1";
        public string? Stream { get; set; } = "1";
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Day { get; set; }
        public string? PeriodFrom { get; set; }
    }

    public class UpdateEFromDateRequest
    {
        public string? Shift { get; set; } = "1";
        public string? Stream { get; set; } = "1";
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public string? Day { get; set; }
        public string? PeriodFrom { get; set; }
        public string? NewEFromDate { get; set; }
        public string? UserId { get; set; }
    }
}
