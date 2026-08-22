using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Admissions
{
    public class StudentInformationReportRequest
    {
        public string AcademicYear { get; set; } = string.Empty;
        public List<string> Columns { get; set; } = new List<string>();
        public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
    }

    public class SaveStudentDataRequest
    {
        public string? Ident { get; set; }
        public string? AdmissionNo { get; set; }
        public string? SName { get; set; }
        public string? AdmissionDate { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Caste { get; set; }
        public string? SubCaste { get; set; }
        public string? MarriedStatus { get; set; }
        public string? Religion { get; set; }
        public string? BloodGroup { get; set; }
        public string? MotherTongue { get; set; }
        public string? StudingYear { get; set; }
        public string? Programme { get; set; }
        public string? Branch { get; set; }
        public string? SecLan { get; set; }
        public string? Medium { get; set; }
        public string? Status { get; set; }
        public string? StudentAadhaar { get; set; }
        public string? MobileNo { get; set; }
        public string? StudentAcNo { get; set; }
        public string? StudentIfscCode { get; set; }
        public string? BankBranchName { get; set; }
        public string? Mole1 { get; set; }
        public string? Mole2 { get; set; }
        public string? RollNum { get; set; }
        public string? Sec { get; set; }
        public string? RationCardNo { get; set; }
        public string? PreYearOfPassing { get; set; }
    }

    public class SaveParentDataRequest
    {
        public string? AdmissionNo { get; set; }
        public string? FName { get; set; }
        public string? FAadhaarNo { get; set; }
        public string? MName { get; set; }
        public string? MMName { get; set; }
        public string? MAadhaarNo { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianAadhaarNo { get; set; }
        public string? AnnualIncomeGuardian { get; set; }
        public string? OccupationOfGuardian { get; set; }
        public string? ParentMobileNo { get; set; }
        public string? Address { get; set; }
        public string? VillageName { get; set; }
        public string? LandlineNo { get; set; }
    }
}

