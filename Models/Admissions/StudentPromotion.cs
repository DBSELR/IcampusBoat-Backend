using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Admissions
{
    public class StudentPromotionRequest
    {
        public string AcademicYear { get; set; } = string.Empty; // Current Academic Year
        public string Course { get; set; } = string.Empty; // CourseCode
        public string Branch { get; set; } = string.Empty; // BranchCode
        public string Year { get; set; } = string.Empty; // Current Studying Year (or "All")
        public string Sem { get; set; } = string.Empty; // Current Semester
        public string PromoteSem { get; set; } = string.Empty; // Promoted Semester
        public string PromoteYear { get; set; } = string.Empty; // Promoted Academic Year
        public string SPromoteYear { get; set; } = string.Empty; // Promoted Studying Year (NewYear)
        public string UserId { get; set; } = string.Empty;
    }

    public class StudentSearchRequest
    {
        public string AcademicYear { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string? BranchCode { get; set; }
        public string? Year { get; set; }
        public string Semester { get; set; } = string.Empty;
    }
}
