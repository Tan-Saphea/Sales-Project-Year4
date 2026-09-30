namespace SALESPURCHASEDB.Models;

public class Member
{
    public int MemberID { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public decimal Discount { get; set; }
}
