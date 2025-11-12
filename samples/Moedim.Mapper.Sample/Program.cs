using Moedim.Mapper;

namespace Moedim.Mapper.Sample;

// Example 1: Basic attribute-based mapping
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

// Example 2: Custom property mapping
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

// Example 3: Ignored properties
public class Employee
{
    public string? Name { get; set; }
    public decimal Salary { get; set; }
    public string? SocialSecurityNumber { get; set; }
}

[MapFrom(typeof(Employee))]
public class EmployeeDto
{
    public string? Name { get; set; }
    
    [IgnoreProperty]
    public decimal Salary { get; set; }
    
    [IgnoreProperty]
    public string? SocialSecurityNumber { get; set; }
}

// Example 4: Collections
public class Team
{
    public string? Name { get; set; }
    public List<string>? Members { get; set; }
}

[MapFrom(typeof(Team))]
public class TeamDto
{
    public string? Name { get; set; }
    public List<string>? Members { get; set; }
}

/// <summary>
/// Sample program demonstrating Moedim.Mapper usage.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Moedim.Mapper Sample Application");
        Console.WriteLine("==================================\n");

        // Example 1: Basic mapping
        var user = new User
        {
            Name = "John Doe",
            Age = 30,
            Email = "john@example.com"
        };

        var userDto = user.ToUserDto();
        Console.WriteLine($"User mapped to DTO: {userDto?.Name}, Age: {userDto?.Age}, Email: {userDto?.Email}");

        // Example 2: Custom property mapping
        var product = new Product
        {
            ProductName = "Laptop",
            Price = 1299.99m
        };

        var productDto = product.ToProductDto();
        Console.WriteLine($"\nProduct mapped with custom property: {productDto?.Name}, Price: ${productDto?.Price}");

        // Example 3: Ignored properties
        var employee = new Employee
        {
            Name = "Jane Smith",
            Salary = 75000m,
            SocialSecurityNumber = "123-45-6789"
        };

        var employeeDto = employee.ToEmployeeDto();
        Console.WriteLine($"\nEmployee mapped (sensitive data ignored): {employeeDto?.Name}");

        // Example 4: Collections
        var team = new Team
        {
            Name = "Development Team",
            Members = new List<string> { "Alice", "Bob", "Charlie" }
        };

        var teamDto = team.ToTeamDto();
        Console.WriteLine($"\nTeam mapped with collection: {teamDto?.Name}, Members: {string.Join(", ", teamDto?.Members ?? new List<string>())}");

        Console.WriteLine("\nAll examples completed successfully!");
    }
}
