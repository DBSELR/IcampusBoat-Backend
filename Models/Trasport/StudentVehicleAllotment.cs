using System;

namespace IcampusBoatBackend.Models.Trasport
{
    public class StudentVehicleAllotmentSaveRequest
    {
        public string Id { get; set; } = "0";
        public string StudentSerialNo { get; set; } = string.Empty;
        public string PresentClass { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string FName { get; set; } = string.Empty;
        public string SYear { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Paid { get; set; } = "0";
        public string Due { get; set; } = "0";
        public string RouteName { get; set; } = string.Empty;
        public string StopPoint { get; set; } = string.Empty;
        public string SName { get; set; } = string.Empty;
        public string VehicleNo { get; set; } = string.Empty;
        public string FromMonth { get; set; } = string.Empty;
        public string ToMonth { get; set; } = string.Empty;
        public string BusFee { get; set; } = "0";
        public string Address { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string FinancialYear { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}
