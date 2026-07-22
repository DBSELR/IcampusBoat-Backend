using System;

namespace IcampusBoatBackend.Models.Trasport
{
    public class RouteMasterIncrementSaveRequest
    {
        public string Id { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public string RouteIncrement { get; set; } = "0";
        public string? UserId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public string FinancialYear { get; set; } = string.Empty;
    }
}
