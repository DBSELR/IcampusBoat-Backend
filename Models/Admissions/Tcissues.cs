using System;

namespace IcampusBoatBackend.Models.Admissions
{
    public class Tcissues
    {
        public string? Tid { get; set; } = "0";
        public string SSNO { get; set; } = string.Empty; // Registration No
        public string? TCNo { get; set; }
        public string? DateOfAdmission { get; set; }
        public string? StudentName { get; set; }
        public string? Fname { get; set; }
        public string? DOB { get; set; }
        public string? Religion { get; set; }
        public string? Caste { get; set; }
        public string? SubCaste { get; set; }
        public string? ClassofLeaving { get; set; }
        public string? Group { get; set; }
        public string? Course { get; set; }
        public string? FeeDue { get; set; }
        public string? Nationality { get; set; }
        public string? MotherTongue { get; set; }
        public string? TCDate { get; set; }
        public string? Conduct { get; set; }
        public string? ReasonForLeaving { get; set; }
        public string? DateofLeaving { get; set; }
        public string? Mole1 { get; set; }
        public string? Mole2 { get; set; }
        public string? University { get; set; }
        public string? ADMNO { get; set; }
        public string? AcademicYear { get; set; }
        public string? Scholar { get; set; }
        public string? Qualified { get; set; }
    }

    public class StudentTcDetailsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? StudentData { get; set; }
        public int CalculatedFeeDue { get; set; }
        public bool HasFeeDueBlock { get; set; }
        public string? FeeDueWarningMessage { get; set; }
    }
}
