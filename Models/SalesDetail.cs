namespace SALESPURCHASEDB.Models;

public class SalesDetail
{
    public int SalesID { get; set; }
    public int ProductID { get; set; }
    public int QtySales { get; set; }

    // Display properties
    public string? ProductName { get; set; }
    public decimal? UnitPriceOut { get; set; }
}
