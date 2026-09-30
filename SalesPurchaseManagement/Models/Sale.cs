using System;
using System.Collections.Generic;

namespace SalesPurchaseManagement.Models;

/// <summary>
/// Represents a sales transaction header from tblSales.
/// </summary>
public class Sale
{
    public int SalesID { get; set; }
    public int EmployeeID { get; set; }
    public string? EmployeeName { get; set; }
    public int? CustomerID { get; set; }
    public string? CustomerName { get; set; }
    public DateTime SalesDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<SalesDetail> SalesDetails { get; set; } = new();
}
