using System;

namespace IcampusBoatBackend.Models.Admissions
{
    public class ExpenditureMasterSaveRequest
    {
        public string? Id { get; set; } = "0";
        public string Course { get; set; } = string.Empty; // CourseCode
        public string Year { get; set; } = string.Empty;
        public string ExpenditureHeads { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
    }
}
