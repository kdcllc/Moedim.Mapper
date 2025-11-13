namespace Moedim.Mapper.Sample.Models;

/// <summary>
/// Represents an item in an order.
/// </summary>
public class OrderItem
{
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
}
