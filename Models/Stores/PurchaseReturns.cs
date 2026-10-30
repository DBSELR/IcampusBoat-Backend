namespace IcampusBoatBackend.Models.Stores
{
    public class SavePurchaseReturnRequest
    {
        public string? Id { get; set; }
        public string? AcademicYear { get; set; }
        public string? FinancialYear { get; set; }
        public string? BillNo { get; set; }
        public string? ItemsName { get; set; }
        public string? SupplierName { get; set; }
        public string? PQty { get; set; }
        public string? RQty { get; set; }
        public string? Remarks { get; set; }
        public string? UserId { get; set; }
    }
}
