using System;

namespace IcampusBoatBackend.Models.Admissions
{
    public class NocSaveRequest
    {
        public string? Id { get; set; } = "0";
        public string NocNo { get; set; } = string.Empty;
        public string? Date { get; set; }
        public string RegNo { get; set; } = string.Empty; // Registration No / txtSSNO
        public string? AdmissionDate { get; set; }
        public string? StudentName { get; set; }
        public string? FatherName { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Year { get; set; }
        public string? FromStudentTransfe { get; set; } // txtFromSeeking
        public string? ToStudentTransfe { get; set; } // txtToSeeking
        public string? AffiliatingUniversity { get; set; }
        public string? UniversityissuedtheNOC { get; set; } // ddlYESNO
        public string? TotalintakeinIYear { get; set; }
        public string? Quota { get; set; }
        public string? Annualtuitionfee { get; set; }
        public string? TuitionfeeChargeble { get; set; }
        public string? ReasonForTransfer { get; set; } // ddlRsnTrnsfer
        public string AcademicYear { get; set; } = string.Empty;
        public string? Principal { get; set; }
        public string? JAccyr { get; set; }
        public string? DateMonthlastExamination { get; set; } // TxtMY
        public string? DetailsDiscontinue { get; set; } // txtDOD
        public string? SeekingTransfer { get; set; } // txtClassToTransfer
        public string? SeekingTransfer2 { get; set; } // txtClassToTransfer2
        public string? NoOfUnfilled { get; set; } // txtNoUnfilled
        public string? StydyYear { get; set; } // txtClassToStudied
        public string? StydyDetails { get; set; } // txtClassToStudied2
        public string? Takenaccyr { get; set; }
        public string? Noofunfilledseatsaccyr { get; set; }
    }
}
