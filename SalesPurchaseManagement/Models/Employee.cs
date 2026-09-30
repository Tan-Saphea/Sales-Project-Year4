using System;

namespace SalesPurchaseManagement.Models;

/// <summary>
/// Represents an employee record from tblEmployees.
/// </summary>
public class Employee
{
    public int EmployeeID { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? UserType { get; set; }
    public string? Role { get; set; }
    public string? JobTitle { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? HomePhone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? StateProvince { get; set; }
    public string? ZipPostalCode { get; set; }
    public string? CountryRegion { get; set; }
    public byte[]? Photo { get; set; }
}
