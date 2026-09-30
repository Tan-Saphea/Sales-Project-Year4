using System;

namespace SalesPurchaseManagement.Models;

/// <summary>
/// Represents a sales transaction line item from tblSalesDetails.
/// </summary>
public class SalesDetail
{
    public int SalesID { get; set; }
    public int ProductID { get; set; }
    public string? ProductName { get; set; }
    public decimal QtySales { get; set; }
    public decimal UnitPriceOut { get; set; }
    public decimal Subtotal => QtySales * UnitPriceOut;
}
