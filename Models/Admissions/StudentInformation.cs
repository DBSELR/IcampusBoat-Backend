using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Admissions
{
    public class StudentInformationReportRequest
    {
        public string AcademicYear { get; set; } = string.Empty;
        public List<string> Columns { get; set; } = new List<string>();
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
    }
}
