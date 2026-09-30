using System;

namespace SALESPURCHASEDB.Models;

public class Purchase
{
    public int PurchaseID { get; set; }
    public int EmployeeID { get; set; }
    public int SupplierID { get; set; }
    public int? ShipperID { get; set; }
    public DateTime PurchaseDate { get; set; }

    // Display properties
    public string? EmployeeName { get; set; }
    public string? SupplierName { get; set; }
    public string? ShipperName { get; set; }
}
