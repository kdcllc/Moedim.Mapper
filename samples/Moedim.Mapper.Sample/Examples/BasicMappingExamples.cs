using Moedim.Mapper;

namespace Moedim.Mapper.Sample.Examples;

#region Basic Models

public class User
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
}

[MapFrom(typeof(User))]
public class UserDto
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
}

public class Product
{
    public string? ProductName { get; set; }
    public decimal Price { get; set; }
}

[MapFrom(typeof(Product))]
public class ProductDto
{
    [MapProperty("ProductName")]
    public string? Name { get; set; }
    public decimal Price { get; set; }
}

#endregion

/// <summary>
/// Demonstrates basic mapping scenarios.
/// </summary>
public static class BasicMappingExamples
{
    public static void Run()
    {
        Console.WriteLine("=== Basic Mapping Examples ===\n");

        // Example 1: Simple property mapping
        var user = new User
        {
            Name = "John Doe",
            Age = 30,
            Email = "john@example.com"
        };

        var userDto = user.ToUserDto();
        Console.WriteLine($"1. Simple Mapping:");
        Console.WriteLine($"   User: {userDto?.Name}, Age: {userDto?.Age}, Email: {userDto?.Email}\n");

        // Example 2: Custom property mapping
        var product = new Product
        {
            ProductName = "Laptop",
            Price = 1299.99m
        };

        var productDto = product.ToProductDto();
        Console.WriteLine($"2. Custom Property Mapping:");
        Console.WriteLine($"   Product: {productDto?.Name}, Price: ${productDto?.Price:F2}\n");
    }
}
