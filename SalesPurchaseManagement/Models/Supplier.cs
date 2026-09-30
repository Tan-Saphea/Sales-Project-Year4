using System;

namespace SalesPurchaseManagement.Models;

/// <summary>
/// Represents a supplier record from tblSuppliers.
/// </summary>
public class Supplier
{
    public int SupplierID { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? CompanyName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Country { get; set; }
    public string? Website { get; set; }
}
