namespace IcampusBoatBackend.Models.Stores
{
    public class SaveStockInwardRequest
    {
        public string? Id { get; set; }
        public string? AcademicYear { get; set; }
        public string? FinancialYear { get; set; }
        public string? BillNo { get; set; }
        public string? BillDate { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierAddress { get; set; }
        public string? ItemsName { get; set; }
        public string? ProdCode { get; set; }
        public string? Quantity { get; set; }
        public string? PurchasePrice { get; set; }
        public string? Amount { get; set; }
        public string? UserId { get; set; }
        public string? Cgst { get; set; } = "0";
        public string? Sgst { get; set; } = "0";
        public string? Igst { get; set; } = "0";
        public string? HsnCode { get; set; } = "0";
        public string? TotalTaxAmount { get; set; } = "0";
        public string? TotalAmount { get; set; } = "0";
        public string? TaxType { get; set; }
    }

    public class CalculateStockInwardTaxRequest
    {
        public string? TaxType { get; set; }
        public string? Quantity { get; set; }
        public string? PurchasePrice { get; set; }
        public string? Cgst { get; set; } = "0";
        public string? Sgst { get; set; } = "0";
        public string? Igst { get; set; } = "0";
        public string? HsnCode { get; set; } = "0";
    }
}
