using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Attendance
{
    public class AdminPermissionsSectionRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? AcdYr { get; set; }
    }

    public class NoAttendanceLecturersRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Section { get; set; }
        public string? Day { get; set; }
        public string? Semester { get; set; }
        public string? SYear { get; set; }
        public string? AcdYr { get; set; }
        public string? Date { get; set; }
        public string? ToDate { get; set; }
    }

    public class FacultyGridRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Section { get; set; }
        public string? Semester { get; set; }
        public string? SYear { get; set; }
        public string? AcademicYear { get; set; }
        public string? Date { get; set; }
    }

    public class PermissionItem
    {
        public string? Lecturer { get; set; }
        public string? Subject { get; set; }
        public bool Granted { get; set; }
    }

    public class SaveAdminPermissionsBatchRequest
    {
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SYear { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; } = "0";
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? AcdYr { get; set; }
        public List<PermissionItem>? Permissions { get; set; }
    }
}
