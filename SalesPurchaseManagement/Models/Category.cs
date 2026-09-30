using System;

namespace SalesPurchaseManagement.Models;

/// <summary>
/// Represents a product category record from tblCategories.
/// </summary>
public class Category
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? CreatedDate { get; set; }
    public int? CreatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public DateTime? UpdateDate { get; set; }
    public int? UpdateBy { get; set; }
}
