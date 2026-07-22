using System;

namespace IcampusBoatBackend.Models.Admissions
{
    public class SectionandRollNumSaveRequest
    {
        public string? Cid { get; set; } = string.Empty;
        public string StudyingYear { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty; // CourseCode
        public string Branch { get; set; } = string.Empty; // BranchCode
        public string SectionCapacity { get; set; } = string.Empty;
        public string Alloted { get; set; } = string.Empty;
        public string RollNoStartFrom { get; set; } = string.Empty;
        public string RollNoEnd { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Stream { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
    }

    public class GetSectionsRequest
    {
        public string Course { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string StudyingYear { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
    }

    public class CapacityCheckRequest
    {
        public string Course { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string StudyingYear { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
    }

    public class AllottedCheckRequest
    {
        public string Course { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string StudyingYear { get; set; } = string.Empty;
    }
}
