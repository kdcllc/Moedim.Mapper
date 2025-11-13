using Moedim.Mapper;
using Moedim.Mapper.Sample.Models;

namespace Moedim.Mapper.Sample.DTOs;

/// <summary>
/// Data transfer object for Order with collection of items.
/// </summary>
[MapFrom(typeof(Order))]
public class OrderDto
{
    public string? OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public PersonDto? Customer { get; set; }
    public List<OrderItemDto>? Items { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Status { get; set; }

    public int ItemCount => Items?.Count ?? 0;
}
