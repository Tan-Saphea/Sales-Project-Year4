using System;

namespace SalesPurchaseManagement.Models;

/// <summary>
/// Represents a shipping company record from tblShippers.
/// </summary>
public class Shipper
{
    public int ShipperID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? ContactName { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Country { get; set; }
}
