using System;

namespace IcampusBoatBackend.Models.Admissions
{
    public class GroupChangeSaveRequest
    {
        public string? Hid { get; set; } = "0";
        public string? Date { get; set; }
        public string? ReceiptNo { get; set; }
        public string RegNo { get; set; } = string.Empty; // Registration No / TxtAdmino
        public string? StudentName { get; set; }
        public string? Course { get; set; } // Current Course Code (lblPrgcode)
        public string? Branch { get; set; } // Current Branch Code (lblBrachCode)
        public string ChangedGroup { get; set; } = string.Empty; // Selected Branch Code
        public string? RollNo { get; set; }
        public string Section { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty; // Studying Year (txtYr)
        public string NewRegNo { get; set; } = string.Empty;
        public string? Remarks { get; set; }
    }
}
