using System;

namespace IcampusBoatBackend.Models.Admissions
{
    public class AdmissionsViewSaveRequest
    {
        public string Ident { get; set; } = "0";
        public string StudentSerialNo { get; set; } = string.Empty;
        public string AdmNo { get; set; } = string.Empty;
        public string RegistrationNo { get; set; } = string.Empty;
        public string? AdmissionDate { get; set; }
        public string? DOB { get; set; }
        public string SName { get; set; } = string.Empty;
        public string? ModeofAdm { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? Section { get; set; }
        public string AYear { get; set; } = "0";
        public string SYear { get; set; } = "0";
        public string? AcadamicYear { get; set; }
        public string? JAcadamicYear { get; set; }
        public string ASemester { get; set; } = "0";
        public string SSemester { get; set; } = "0";
        public string? Caste { get; set; }
        public string? SubCaste { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public string? Religion { get; set; }
        public string? BloodGrp { get; set; }
        public string? PH { get; set; }
        public string TuitionFee { get; set; } = "0";
        public string Miscellaneousfee { get; set; } = "0";
        public string SchAmount { get; set; } = "0";
        public string BHFee { get; set; } = "0";
        public string LHFee { get; set; } = "0";
        public string BusFee { get; set; } = "0";
        public string Donation { get; set; } = "0";
        public string? Rank { get; set; }
        public string? HallTicketNo { get; set; }
        public string? SSCSchoolName { get; set; }
        public string? SSCMarksPercentage { get; set; }
        public string? LastAttendedCollegeName { get; set; }
        public string? GroupSubjectsMarksPercentage { get; set; }
        public string? Aggregate { get; set; }
        public string? MYPassing { get; set; }
        public string? FName { get; set; }
        public string? ParentOccupation { get; set; }
        public string? Income { get; set; } = "0";
        public string? MName { get; set; }
        public string? Address { get; set; }
        public string? ParentMbNo { get; set; }
        public string? StdMobNo { get; set; }
        public string? AadhaarNo { get; set; }
        public string? RationcardNo { get; set; }
        public string? Emailid { get; set; }
        public string? UserId { get; set; }
        public string STATUS { get; set; } = "INSERT";
        public string? SSC_HallTicketNo { get; set; }
        public string? SSC_Board { get; set; }
        public string? SSCStudied { get; set; }
        public string? SSC_Aggregate { get; set; }
        public string? SSC_MYPassing { get; set; }
        public string? Int_CollegeName { get; set; }
        public string? Int_MarksPerc { get; set; }
        public string? Int_HallTicketNo { get; set; }
        public string? Int_Board { get; set; }
        public string? Int_Aggregate { get; set; }
        public string? Int_MYPassing { get; set; }
        public string? UG_CollegeName { get; set; }
        public string? UG_MarksPerc { get; set; }
        public string? UG_HallTicketNo { get; set; }
        public string? UG_University { get; set; }
        public string? UG_Aggregate { get; set; }
        public string? UG_MYPassing { get; set; }
        public string? Fee_admType { get; set; }
        public bool Isactive { get; set; } = true;
        public string? Reason { get; set; }
        public string? Date { get; set; } // active status date
        public string? AStatus { get; set; }
        public string? SET { get; set; }
        public string? HallTicket { get; set; }
        public string? SETRank { get; set; }
        public string? BranchRank { get; set; }
        public string? Mole1 { get; set; }
        public string? Mole2 { get; set; }
        public string? States { get; set; }
        public string? Category { get; set; }
        public string? RoutePoint { get; set; }
        public string? ParentMbNo2 { get; set; }
        public string? ICNo { get; set; }
        public string? MotherTongue { get; set; }
        public string? Maths { get; set; }
        public string? Physics { get; set; }
        public string? Chemistry { get; set; }
        public string SpotAdmFee { get; set; } = "0";
        public bool LE { get; set; }
        public bool Fac_Child { get; set; }
        public string? JnanaBhumiId { get; set; }
        public string? Regulation { get; set; }
        public string? MAadharNo { get; set; }
        public string? UgCourse { get; set; }
        public string? LIBRARYMEMBERGROUP { get; set; }
        public bool SCHLOR { get; set; }
        public string? ModeofCtgy { get; set; }
        public string? AllottedQuota { get; set; }
        public bool NSP { get; set; }
        public string? APAAR { get; set; }
    }
}
