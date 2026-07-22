using System;

namespace IcampusBoatBackend.Models.Trasport
{
    public class MeterReadingSaveRequest
    {
        public string Id { get; set; } = "0";
        public string RouteName { get; set; } = string.Empty;
        public string VehicleNo { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty; // dd-MM-yyyy
        public string OpeningMeterReading { get; set; } = "0";
        public string ClosingMeterReading { get; set; } = "0";
        public string Remarks { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string FinancialYear { get; set; } = string.Empty;
    }
}
