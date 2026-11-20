
namespace SalesAnalytics.Models;

public class Sale
{
    public int SaleID { get; set; }
    public DateTime SaleDate { get; set; }
    public string CustomerName { get; set; } = "";
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal TotalAmount => Quantity * UnitPrice;
}
