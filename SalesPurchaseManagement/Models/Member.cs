using System;

namespace SalesPurchaseManagement.Models;

/// <summary>
/// Represents a membership tier record from tblMembers.
/// </summary>
public class Member
{
    public int MemberID { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public decimal Discount { get; set; }
}
