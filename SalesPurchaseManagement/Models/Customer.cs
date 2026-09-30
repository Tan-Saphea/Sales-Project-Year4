using System;

namespace SalesPurchaseManagement.Models;

/// <summary>
/// Represents a customer record from tblCustomers with joined membership information.
/// </summary>
public class Customer
{
    public int CustomerID { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int? MemberID { get; set; }

    // Joined fields from tblMembers
    public string? MemberName { get; set; }
    public decimal Discount { get; set; }
}
