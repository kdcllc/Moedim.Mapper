using FluentAssertions;
using Xunit;

namespace Moedim.Mapper.Tests;

/// <summary>
/// Source class for custom property mapping tests.
/// </summary>
public class Product
{
    public string? ProductName { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

/// <summary>
/// DTO with custom property mapping.
/// </summary>
[MapFrom(typeof(Product))]
public class ProductDto
{
    [MapProperty("ProductName")]
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

/// <summary>
/// Source class for ignored property tests.
/// </summary>
public class Employee
{
    public string? Name { get; set; }
    public decimal Salary { get; set; }
    public string? Department { get; set; }
}

/// <summary>
/// DTO with ignored properties.
/// </summary>
[MapFrom(typeof(Employee))]
public class EmployeeDto
{
    public string? Name { get; set; }
    
    [IgnoreProperty]
    public decimal Salary { get; set; }
    
    public string? Department { get; set; }
}

/// <summary>
/// Tests for advanced mapping features.
/// </summary>
public class AdvancedMappingTests
{
    [Fact]
    public void Map_WithCustomPropertyName_ShouldMapCorrectly()
    {
        // Arrange
        var product = new Product
        {
            ProductName = "Laptop",
            Price = 1299.99m,
            Stock = 15
        };

        // Act
        var dto = product.ToProductDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Name.Should().Be("Laptop");
        dto.Price.Should().Be(1299.99m);
        dto.Stock.Should().Be(15);
    }

    [Fact]
    public void Map_WithIgnoredProperty_ShouldNotMapIgnoredProperty()
    {
        // Arrange
        var employee = new Employee
        {
            Name = "John Smith",
            Salary = 75000m,
            Department = "Engineering"
        };

        // Act
        var dto = employee.ToEmployeeDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Name.Should().Be("John Smith");
        dto.Department.Should().Be("Engineering");
        dto.Salary.Should().Be(0); // Should remain at default value
    }

    [Fact]
    public void Map_MultipleTimes_ShouldProduceIndependentObjects()
    {
        // Arrange
        var product1 = new Product { ProductName = "Mouse", Price = 29.99m, Stock = 50 };
        var product2 = new Product { ProductName = "Keyboard", Price = 79.99m, Stock = 30 };

        // Act
        var dto1 = product1.ToProductDto();
        var dto2 = product2.ToProductDto();

        // Assert
        dto1.Should().NotBeNull();
        dto2.Should().NotBeNull();
        dto1.Should().NotBeSameAs(dto2);
        dto1!.Name.Should().Be("Mouse");
        dto2!.Name.Should().Be("Keyboard");
    }
}
