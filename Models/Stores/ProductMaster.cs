namespace IcampusBoatBackend.Models.Stores
{
    public class SaveProductMasterRequest
    {
        public string? Id { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? Category { get; set; }
        public string? OpeningStock { get; set; }
        public string? MarketPrice { get; set; }
        public string? AcademicYear { get; set; }
        public string? Course { get; set; }
        public string? ProductType { get; set; } = "A";
        public string? PurchasePrice { get; set; } = "0";
        public string? PurchaseSalesTaxVat { get; set; } = "0";
        public string? PurchaseExcise { get; set; } = "0";
        public string? PurchaseDiscount { get; set; } = "0";
        public string? SalePrice { get; set; } = "0";
        public string? SaleSalesTaxVat { get; set; } = "0";
        public string? SaleExcise { get; set; } = "0";
        public string? SaleDiscount { get; set; } = "0";
        public string? FinancialYear { get; set; }
        public string? MinimumQty { get; set; }
        public string? UserId { get; set; }
        public string? Igst { get; set; } = "0";
        public string? Cgst { get; set; } = "0";
        public string? Sgst { get; set; } = "0";
        public string? HsnCode { get; set; } = "0";
    }

    public class SaveCategoryMasterRequest
    {
        public string? Id { get; set; }
        public string? ProductCode { get; set; }
        public string? Category { get; set; }
        public string? AcademicYear { get; set; }
        public string? UserId { get; set; }
    }
}
