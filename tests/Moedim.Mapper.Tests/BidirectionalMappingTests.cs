using FluentAssertions;
using Xunit;

namespace Moedim.Mapper.Tests;

/// <summary>
/// Source class for bidirectional mapping.
/// </summary>
public class Order
{
    public int Id { get; set; }
    public string? OrderNumber { get; set; }
    public decimal Total { get; set; }
}

/// <summary>
/// DTO class with both directions configured.
/// </summary>
[MapFrom(typeof(Order))]
[MapTo(typeof(Order))]
public class OrderDto
{
    public int Id { get; set; }
    public string? OrderNumber { get; set; }
    public decimal Total { get; set; }
}

/// <summary>
/// Tests for bidirectional mapping.
/// </summary>
public class BidirectionalMappingTests
{
    [Fact]
    public void Map_OrderToDto_ShouldWork()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            OrderNumber = "ORD-001",
            Total = 599.99m
        };

        // Act
        var dto = order.ToOrderDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Id.Should().Be(1);
        dto.OrderNumber.Should().Be("ORD-001");
        dto.Total.Should().Be(599.99m);
    }

    [Fact]
    public void Map_DtoToOrder_ShouldWork()
    {
        // Arrange
        var dto = new OrderDto
        {
            Id = 2,
            OrderNumber = "ORD-002",
            Total = 799.99m
        };

        // Act
        var order = dto.ToOrder();

        // Assert
        order.Should().NotBeNull();
        order!.Id.Should().Be(2);
        order.OrderNumber.Should().Be("ORD-002");
        order.Total.Should().Be(799.99m);
    }

    [Fact]
    public void Map_RoundTrip_ShouldPreserveData()
    {
        // Arrange
        var original = new Order
        {
            Id = 3,
            OrderNumber = "ORD-003",
            Total = 1299.99m
        };

        // Act
        var dto = original.ToOrderDto();
        var roundTrip = dto.ToOrder();

        // Assert
        roundTrip.Should().NotBeNull();
        roundTrip!.Id.Should().Be(original.Id);
        roundTrip.OrderNumber.Should().Be(original.OrderNumber);
        roundTrip.Total.Should().Be(original.Total);
    }
}
