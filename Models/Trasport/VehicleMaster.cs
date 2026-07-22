using System;

namespace IcampusBoatBackend.Models.Trasport
{
    public class VehicleMasterSaveRequest
    {
        public string Id { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public string VehicleNo { get; set; } = string.Empty;
        public string VehicleRegNo { get; set; } = string.Empty;
        public string VehicleCapacity { get; set; } = "0";
        public string DriverName { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string FinancialYear { get; set; } = string.Empty;
    }
}
