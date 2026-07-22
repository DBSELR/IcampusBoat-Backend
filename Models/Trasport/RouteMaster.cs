using System;
using System.Collections.Generic;

namespace IcampusBoatBackend.Models.Trasport
{
    public class RouteMasterSaveRequest
    {
        public string Id { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public string RoutePointOrderNo { get; set; } = string.Empty;
        public string RoutePoint { get; set; } = string.Empty;
        public string BusFee { get; set; } = "0";
        public string StartTime { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string FinancialYear { get; set; } = string.Empty;
        public string? UserId { get; set; }
    }

    public class RoutePointOrderUpdateRequest
    {
        public string Id { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public string RoutePoint { get; set; } = string.Empty;
        public string RoutePointOrderNo { get; set; } = string.Empty;
    }
}
