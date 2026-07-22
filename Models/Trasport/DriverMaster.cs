using System;

namespace IcampusBoatBackend.Models.Trasport
{
    public class DriverMasterSaveRequest
    {
        public string Id { get; set; } = string.Empty;
        public string DriverId { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string Dob { get; set; } = string.Empty; // dd-MM-yyyy
        public string ContactNo { get; set; } = string.Empty;
        public string ReferenceName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Doj { get; set; } = string.Empty; // dd-MM-yyyy
        public string LicenceNo { get; set; } = string.Empty;
        public string LicenceExpireDate { get; set; } = string.Empty; // dd-MM-yyyy
        public string NoOfYearsExperience { get; set; } = "0";
        public string ReferenceContactNo { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string FinancialYear { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
