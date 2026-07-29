using System;

namespace IcampusBoatBackend.Models.Examinations
{
    public class MarksHeadItemModel
    {
        public string? Id { get; set; } = "0";
        public string? HeadCode { get; set; }
        public string? HeadName { get; set; }
        public string? ShortName { get; set; }
        public string? HeadType { get; set; }
        public string? MaxMarks { get; set; }
        public string? MinMarks { get; set; }
        public string? PassMarks { get; set; }
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? Status { get; set; }
        public string? UserId { get; set; }
    }

    public class MarksHeadFilterModel
    {
        public string? AcademicYear { get; set; }
        public string? Programme { get; set; }
        public string? HeadType { get; set; }
        public string? SearchTerm { get; set; }
    }
}
