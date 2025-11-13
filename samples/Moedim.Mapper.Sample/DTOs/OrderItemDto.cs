using Moedim.Mapper;
using Moedim.Mapper.Sample.Models;

namespace Moedim.Mapper.Sample.DTOs;

/// <summary>
/// Data transfer object for OrderItem.
/// </summary>
[MapFrom(typeof(OrderItem))]
public class OrderItemDto
{
    public string? ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
