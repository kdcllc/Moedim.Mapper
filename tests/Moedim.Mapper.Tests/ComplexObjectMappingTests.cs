using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Moedim.Mapper.Tests.ComplexMapping;

#region Test Models - Nested Objects

/// <summary>
/// Organization with nested structure.
/// </summary>
public class Organization
{
    public string? Name { get; set; }
    public Division? PrimaryDivision { get; set; }
    public List<Division>? AllDivisions { get; set; }
}

/// <summary>
/// Division model.
/// </summary>
public class Division
{
    public string? Name { get; set; }
    public Leader? Leader { get; set; }
    public List<TeamMember>? Members { get; set; }
}

/// <summary>
/// Leader model.
/// </summary>
public class Leader
{
    public string? Name { get; set; }
    public int YearsExperience { get; set; }
}

/// <summary>
/// Team member model.
/// </summary>
public class TeamMember
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
}

/// <summary>
/// Organization DTO with nested structure.
/// </summary>
[MapFrom(typeof(Organization))]
public class OrganizationDto
{
    public string? Name { get; set; }
    public DivisionDto? PrimaryDivision { get; set; }
    public List<DivisionDto>? AllDivisions { get; set; }
}

/// <summary>
/// Division DTO.
/// </summary>
[MapFrom(typeof(Division))]
public class DivisionDto
{
    public string? Name { get; set; }
    public LeaderDto? Leader { get; set; }
    public List<TeamMemberDto>? Members { get; set; }
}

/// <summary>
/// Leader DTO.
/// </summary>
[MapFrom(typeof(Leader))]
public class LeaderDto
{
    public string? Name { get; set; }
    public int YearsExperience { get; set; }
}

/// <summary>
/// Team member DTO.
/// </summary>
[MapFrom(typeof(TeamMember))]
public class TeamMemberDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
}

#endregion

#region Test Models - Value Converters

/// <summary>
/// Source model with temperature in Celsius.
/// </summary>
public class WeatherReading
{
    public double Celsius { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// DTO with temperature in Fahrenheit.
/// </summary>
[MapFrom(typeof(WeatherReading))]
public class WeatherReadingDto
{
    [ConvertWith(typeof(CelsiusToFahrenheitConverter))]
    public double Celsius { get; set; }

    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Converter from Celsius to Fahrenheit.
/// </summary>
public class CelsiusToFahrenheitConverter : IValueConverter<double, double>
{
    public double Convert(double celsius)
    {
        return (celsius * 9.0 / 5.0) + 32.0;
    }
}

#endregion

#region Test Models - Collections of Complex Objects

/// <summary>
/// Shopping cart with items.
/// </summary>
public class ShoppingCart
{
    public string? CartId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CartItem>? Items { get; set; }
    public Shopper? Buyer { get; set; }
}

/// <summary>
/// Cart item.
/// </summary>
public class CartItem
{
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

/// <summary>
/// Shopper model.
/// </summary>
public class Shopper
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}

/// <summary>
/// Shopping cart DTO.
/// </summary>
[MapFrom(typeof(ShoppingCart))]
public class ShoppingCartDto
{
    public string? CartId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CartItemDto>? Items { get; set; }
    public ShopperDto? Buyer { get; set; }
}

/// <summary>
/// Cart item DTO.
/// </summary>
[MapFrom(typeof(CartItem))]
public class CartItemDto
{
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

/// <summary>
/// Shopper DTO.
/// </summary>
[MapFrom(typeof(Shopper))]
public class ShopperDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
}

#endregion

/// <summary>
/// Tests for complex object mapping scenarios.
/// </summary>
public class ComplexObjectMappingTests
{
    #region Nested Object Tests

    [Fact]
    public void Map_NestedObject_ShouldMapAllLevels()
    {
        // Arrange
        var org = new Organization
        {
            Name = "Tech Corp",
            PrimaryDivision = new Division
            {
                Name = "Engineering",
                Leader = new Leader
                {
                    Name = "Alice Johnson",
                    YearsExperience = 10
                }
            }
        };

        // Act
        var dto = org.ToOrganizationDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Name.Should().Be("Tech Corp");
        dto.PrimaryDivision.Should().NotBeNull();
        dto.PrimaryDivision!.Name.Should().Be("Engineering");
        dto.PrimaryDivision.Leader.Should().NotBeNull();
        dto.PrimaryDivision.Leader!.Name.Should().Be("Alice Johnson");
        dto.PrimaryDivision.Leader.YearsExperience.Should().Be(10);
    }

    [Fact]
    public void Map_NullNestedObject_ShouldHandleGracefully()
    {
        // Arrange
        var org = new Organization
        {
            Name = "Small Biz",
            PrimaryDivision = null
        };

        // Act
        var dto = org.ToOrganizationDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Name.Should().Be("Small Biz");
        dto.PrimaryDivision.Should().BeNull();
    }

    #endregion

    #region Collection of Complex Objects Tests

    [Fact]
    public void Map_CollectionOfComplexObjects_ShouldMapAllItems()
    {
        // Arrange
        var org = new Organization
        {
            Name = "Multi Dept Corp",
            AllDivisions = new List<Division>
            {
                new Division
                {
                    Name = "Sales",
                    Leader = new Leader { Name = "Bob Smith", YearsExperience = 5 }
                },
                new Division
                {
                    Name = "Marketing",
                    Leader = new Leader { Name = "Carol White", YearsExperience = 7 }
                }
            }
        };

        // Act
        var dto = org.ToOrganizationDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.AllDivisions.Should().NotBeNull();
        dto.AllDivisions.Should().HaveCount(2);
        dto.AllDivisions![0].Name.Should().Be("Sales");
        dto.AllDivisions[0].Leader!.Name.Should().Be("Bob Smith");
        dto.AllDivisions[1].Name.Should().Be("Marketing");
        dto.AllDivisions[1].Leader!.Name.Should().Be("Carol White");
    }

    [Fact]
    public void Map_NestedCollectionOfMembers_ShouldMapAllMembers()
    {
        // Arrange
        var division = new Division
        {
            Name = "Development",
            Members = new List<TeamMember>
            {
                new TeamMember { FirstName = "John", LastName = "Doe", Age = 30 },
                new TeamMember { FirstName = "Jane", LastName = "Smith", Age = 28 },
                new TeamMember { FirstName = "Mike", LastName = "Johnson", Age = 35 }
            }
        };

        // Act
        var dto = division.ToDivisionDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Members.Should().NotBeNull();
        dto.Members.Should().HaveCount(3);
        dto.Members![0].FirstName.Should().Be("John");
        dto.Members[1].FirstName.Should().Be("Jane");
        dto.Members[2].FirstName.Should().Be("Mike");
    }

    [Fact]
    public void Map_CartWithItems_ShouldMapCompleteCart()
    {
        // Arrange
        var cart = new ShoppingCart
        {
            CartId = "CART-12345",
            CreatedAt = new DateTime(2025, 11, 12),
            Buyer = new Shopper
            {
                Name = "David Brown",
                Email = "david@example.com"
            },
            Items = new List<CartItem>
            {
                new CartItem { ProductName = "Laptop", Quantity = 1, Price = 1299.99m },
                new CartItem { ProductName = "Mouse", Quantity = 2, Price = 29.99m }
            }
        };

        // Act
        var dto = cart.ToShoppingCartDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.CartId.Should().Be("CART-12345");
        dto.CreatedAt.Should().Be(new DateTime(2025, 11, 12));
        dto.Buyer.Should().NotBeNull();
        dto.Buyer!.Name.Should().Be("David Brown");
        dto.Items.Should().HaveCount(2);
        dto.Items![0].ProductName.Should().Be("Laptop");
        dto.Items[0].Quantity.Should().Be(1);
        dto.Items[1].ProductName.Should().Be("Mouse");
        dto.Items[1].Quantity.Should().Be(2);
    }

    [Fact]
    public void Map_EmptyCollection_ShouldMapToEmptyList()
    {
        // Arrange
        var division = new Division
        {
            Name = "Empty Dept",
            Members = new List<TeamMember>()
        };

        // Act
        var dto = division.ToDivisionDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Members.Should().NotBeNull();
        dto.Members.Should().BeEmpty();
    }

    #endregion

    #region Value Converter Tests

    [Fact]
    public void Map_WithValueConverter_ShouldConvertValue()
    {
        // Arrange
        var reading = new WeatherReading
        {
            Celsius = 25.0,
            Timestamp = DateTime.Now
        };

        // Act
        var dto = reading.ToWeatherReadingDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Celsius.Should().BeApproximately(77.0, 0.1); // 25°C = 77°F
        dto.Timestamp.Should().Be(reading.Timestamp);
    }

    [Fact]
    public void Map_WithValueConverter_FreezingPoint_ShouldConvertCorrectly()
    {
        // Arrange
        var reading = new WeatherReading
        {
            Celsius = 0.0,
            Timestamp = DateTime.Now
        };

        // Act
        var dto = reading.ToWeatherReadingDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Celsius.Should().BeApproximately(32.0, 0.1); // 0°C = 32°F
    }

    #endregion

    #region Deep Nesting Tests

    [Fact]
    public void Map_DeeplyNestedStructure_ShouldMapAllLevels()
    {
        // Arrange
        var org = new Organization
        {
            Name = "Enterprise Corp",
            PrimaryDivision = new Division
            {
                Name = "IT",
                Leader = new Leader
                {
                    Name = "Sarah Connor",
                    YearsExperience = 15
                },
                Members = new List<TeamMember>
                {
                    new TeamMember { FirstName = "Kyle", LastName = "Reese", Age = 28 },
                    new TeamMember { FirstName = "John", LastName = "Connor", Age = 25 }
                }
            },
            AllDivisions = new List<Division>
            {
                new Division
                {
                    Name = "Operations",
                    Leader = new Leader { Name = "T-800", YearsExperience = 20 },
                    Members = new List<TeamMember>
                    {
                        new TeamMember { FirstName = "Miles", LastName = "Dyson", Age = 40 }
                    }
                }
            }
        };

        // Act
        var dto = org.ToOrganizationDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Name.Should().Be("Enterprise Corp");

        // Check primary division
        dto.PrimaryDivision.Should().NotBeNull();
        dto.PrimaryDivision!.Leader.Should().NotBeNull();
        dto.PrimaryDivision.Leader!.Name.Should().Be("Sarah Connor");
        dto.PrimaryDivision.Members.Should().HaveCount(2);
        dto.PrimaryDivision.Members![0].FirstName.Should().Be("Kyle");

        // Check divisions collection
        dto.AllDivisions.Should().HaveCount(1);
        dto.AllDivisions![0].Leader.Should().NotBeNull();
        dto.AllDivisions[0].Leader!.Name.Should().Be("T-800");
        dto.AllDivisions[0].Members.Should().HaveCount(1);
        dto.AllDivisions[0].Members![0].FirstName.Should().Be("Miles");
    }

    #endregion
}
