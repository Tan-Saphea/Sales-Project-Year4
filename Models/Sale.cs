using System;

namespace SALESPURCHASEDB.Models;

public class Sale
{
    public int SalesID { get; set; }
    public int EmployeeID { get; set; }
    public int? CustomerID { get; set; }
    public DateTime SalesDate { get; set; }

    // Display properties
    public string? EmployeeName { get; set; }
    public string? CustomerName { get; set; }
}
