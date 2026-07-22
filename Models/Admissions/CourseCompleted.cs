using System;

namespace IcampusBoatBackend.Models.Admissions
{
    public class CourseCompletedSaveRequest
    {
        public string? Id { get; set; } = "0";
        public string SCNO { get; set; } = string.Empty;
        public string? Date { get; set; }
        public string RegNo { get; set; } = string.Empty; // Registration No / txtREGNO
        public string? AdmissionDate { get; set; }
        public string? StudentName { get; set; }
        public string? FatherName { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? FromAcademicYear { get; set; }
        public string? ToAcademicYear { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string? SCType { get; set; } // ddlType
        public string? Conduct { get; set; } // ddlconduct
    }
}
