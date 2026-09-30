using System;

namespace SalesPurchaseManagement.Models;

/// <summary>
/// Represents a purchase order line item from tblPurchaseDetails.
/// </summary>
public class PurchaseDetail
{
    public int PurchaseID { get; set; }
    public int ProductID { get; set; }
    public string? ProductName { get; set; }
    public decimal PurchaseQty { get; set; }
    public decimal UnitPriceIn { get; set; }
    public decimal Subtotal => PurchaseQty * UnitPriceIn;
}
