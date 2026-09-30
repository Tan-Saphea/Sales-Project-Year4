namespace SALESPURCHASEDB.Models;

public class PurchaseDetail
{
    public int PurchaseID { get; set; }
    public int ProductID { get; set; }
    public int PurchaseQty { get; set; }

    // Display properties
    public string? ProductName { get; set; }
    public decimal? UnitPriceIn { get; set; }
}
