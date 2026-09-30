using System;

namespace SalesPurchaseManagement.Models;

/// <summary>
/// Represents a product record from tblProducts.
/// </summary>
public class Product
{
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryID { get; set; }
    public string? CategoryName { get; set; }
    public string? Barcode { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal Qty { get; set; }
    public decimal UnitPriceIn { get; set; }
    public decimal UnitPriceOut { get; set; }
    public byte[]? ProductImage { get; set; }
    public bool IsDeleted { get; set; }
}
