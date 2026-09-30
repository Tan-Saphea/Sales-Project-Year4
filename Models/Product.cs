using System;

namespace SALESPURCHASEDB.Models;

public class Product
{
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryID { get; set; }
    public string? Barcode { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Qty { get; set; }
    public decimal UnitPriceIn { get; set; }
    public decimal UnitPriceOut { get; set; }
    public byte[]? ProductImage { get; set; }
    public int IsDeleted { get; set; }

    // Display property
    public string? CategoryName { get; set; }
}
