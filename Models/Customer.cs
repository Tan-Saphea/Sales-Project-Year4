namespace SALESPURCHASEDB.Models;

public class Customer
{
    public int CustomerID { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int? MemberID { get; set; }

    // Display property
    public string? MemberName { get; set; }
}
