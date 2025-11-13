namespace Moedim.Mapper.Sample.Models;

/// <summary>
/// Represents an order with collection of items.
/// </summary>
public class Order
{
    public string? OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public Person? Customer { get; set; }
    public List<OrderItem>? Items { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }
}
