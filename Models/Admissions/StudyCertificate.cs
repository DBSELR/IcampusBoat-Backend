using System;

namespace IcampusBoatBackend.Models.Admissions
{
    public class StudyCertificateSaveRequest
    {
        public string? Id { get; set; } = "0";
        public string SCNO { get; set; } = string.Empty;
        public string? Date { get; set; }
        public string RegNo { get; set; } = string.Empty; // Registration No / txtSSno
        public string? AdmissionDate { get; set; }
        public string? StudentName { get; set; }
        public string? FatherName { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? SCType { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string? FACYR { get; set; }
        public string? TACYR { get; set; }
        public string? Conduct { get; set; }
        public string? Type { get; set; }
        public string? Purpose { get; set; }
    }
}
