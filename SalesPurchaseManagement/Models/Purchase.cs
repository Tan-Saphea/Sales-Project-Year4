using System;
using System.Collections.Generic;

namespace SalesPurchaseManagement.Models;

/// <summary>
/// Represents a purchase order header from tblPurchase.
/// </summary>
public class Purchase
{
    public int PurchaseID { get; set; }
    public int EmployeeID { get; set; }
    public string? EmployeeName { get; set; }
    public int SupplierID { get; set; }
    public string? SupplierName { get; set; }
    public int? ShipperID { get; set; }
    public string? ShipperName { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<PurchaseDetail> PurchaseDetails { get; set; } = new();
}
