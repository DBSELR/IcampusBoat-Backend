using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Admissions
{
    public class DelInActiveStudentDeleteRequest
    {
        public string Programme { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string SYear { get; set; } = string.Empty;
        public string Semester { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public List<string> RegNos { get; set; } = new List<string>();
        public string? UserId { get; set; }
    }
}
