# Moedim.Mapper

C# Source Generator Object Mapper - Convention-based and Attribute-based mapping with compile-time code generation

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)]()
[![NuGet](https://img.shields.io/badge/nuget-v1.0.0-blue)]()
[![License](https://img.shields.io/badge/license-MIT-green)]()

![Stand With Israel](./img/IStandWithIsrael.png)

> This is a Hebrew word that translates "feast" or "appointed time."
> "Appointed times" refers to HaSham's festivals in Vayikra/Leviticus 23rd.
> The feasts are "signals and signs" to help us know what is on the heart of HaShem.

## Features

- **Compile-Time Code Generation** - Zero runtime overhead with source generators
- **Attribute-Based Mapping** - Simple, declarative mapping configuration
- **Type-Safe** - Full compile-time type checking and IntelliSense support
- **Null-Safe** - Handles nullable reference types correctly
- **Nested Object Mapping** - Automatically maps nested complex objects
- **Collection Support** - Maps List, Array, IEnumerable with complex object transformation
- **Custom Property Mapping** - Map properties with different names
- **Value Converters** - Custom transformation logic for property values
- **Ignored Properties** - Exclude specific properties from mapping
- **High Performance** - Faster than reflection-based mappers
- **Multi-Framework Support** - Targets .NET 6.0, .NET 7.0, and .NET 8.0

## Hire me

Please send [email](mailto:info@kingdavidconsulting.com) if you consider to **hire me**.

[![buymeacoffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/vyve0og)

## Give a Star! :star:

If you like or are using this project to learn or start your solution, please give it a star. Thanks!

## Installation

```bash
dotnet add package Moedim.Mapper
```

## Quick Start

### Basic Attribute-Based Mapping

```csharp
using Moedim.Mapper;

// Source class
public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
}

// Destination class with MapFrom attribute
[MapFrom(typeof(User))]
public class UserDto
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
}

// Usage
var user = new User { Name = "John", Age = 30, Email = "john@example.com" };
var dto = user.ToUserDto(); // Extension method generated automatically
```

### Custom Property Mapping

```csharp
public class Product
{
    public string ProductName { get; set; }
    public decimal Price { get; set; }
}

[MapFrom(typeof(Product))]
public class ProductDto
{
    [MapProperty("ProductName")]  // Map from different property name
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// Usage
var product = new Product { ProductName = "Laptop", Price = 1299.99m };
var dto = product.ToProductDto();
// dto.Name will contain "Laptop"
```

### Ignoring Properties

```csharp
public class Employee
{
    public string Name { get; set; }
    public decimal Salary { get; set; }
    public string SocialSecurityNumber { get; set; }
}

[MapFrom(typeof(Employee))]
public class EmployeeDto
{
    public string Name { get; set; }

    [IgnoreProperty]  // Excluded from mapping
    public decimal Salary { get; set; }

    [IgnoreProperty]  // Excluded from mapping
    public string SocialSecurityNumber { get; set; }
}
```

### Collection Mapping

```csharp
public class Team
{
    public string Name { get; set; }
    public List<string> Members { get; set; }
}

[MapFrom(typeof(Team))]
public class TeamDto
{
    public string Name { get; set; }
    public List<string> Members { get; set; }
}

// Collections are mapped automatically
var team = new Team
{
    Name = "Dev Team",
    Members = new List<string> { "Alice", "Bob" }
};
var dto = team.ToTeamDto();
```

### Bidirectional Mapping

```csharp
// Map in both directions
[MapTo(typeof(PersonDto))]
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

[MapFrom(typeof(Person))]
public class PersonDto
{
    public string Name { get; set; }
    public int Age { get; set; }
}

// Now both directions work
var person = new Person { Name = "Alice", Age = 25 };
var dto = person.ToPersonDto();
var personAgain = dto.ToPerson();
```

## Complex Object Mapping

### Nested Object Mapping

Moedim.Mapper automatically handles nested complex objects:

```csharp
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
}

[MapFrom(typeof(Address))]
public class AddressDto
{
    public string Street { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
}

public class Customer
{
    public string Name { get; set; }
    public Address DeliveryAddress { get; set; }
}

[MapFrom(typeof(Customer))]
public class CustomerDto
{
    public string Name { get; set; }
    public AddressDto DeliveryAddress { get; set; }
}

// Nested objects are automatically mapped
var customer = new Customer
{
    Name = "John Doe",
    DeliveryAddress = new Address
    {
        Street = "123 Main St",
        City = "New York",
        ZipCode = "10001"
    }
};

var dto = customer.ToCustomerDto();
// dto.DeliveryAddress will be an AddressDto with all properties mapped
```

### Collections of Complex Objects

Map collections containing complex objects:

```csharp
public class OrderItem
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

[MapFrom(typeof(OrderItem))]
public class OrderItemDto
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class Order
{
    public string OrderNumber { get; set; }
    public List<OrderItem> Items { get; set; }
}

[MapFrom(typeof(Order))]
public class OrderDto
{
    public string OrderNumber { get; set; }
    public List<OrderItemDto> Items { get; set; }
}

// Collection items are automatically transformed
var order = new Order
{
    OrderNumber = "ORD-001",
    Items = new List<OrderItem>
    {
        new OrderItem { ProductName = "Laptop", Quantity = 1, Price = 1299.99m },
        new OrderItem { ProductName = "Mouse", Quantity = 2, Price = 29.99m }
    }
};

var dto = order.ToOrderDto();
// dto.Items will be a List<OrderItemDto> with all items mapped
```

### Value Converters

Apply custom transformation logic to property values:

```csharp
public class TemperatureReading
{
    public double Celsius { get; set; }
    public DateTime Timestamp { get; set; }
}

// Custom converter implementation
public class CelsiusToFahrenheitConverter : IValueConverter<double, double>
{
    public double Convert(double celsius)
    {
        return (celsius * 9.0 / 5.0) + 32.0;
    }
}

[MapFrom(typeof(TemperatureReading))]
public class TemperatureReadingDto
{
    [ConvertWith(typeof(CelsiusToFahrenheitConverter))]
    public double Celsius { get; set; }  // Will contain Fahrenheit value

    public DateTime Timestamp { get; set; }
}

// Usage
var reading = new TemperatureReading { Celsius = 25.0, Timestamp = DateTime.Now };
var dto = reading.ToTemperatureReadingDto();
// dto.Celsius will be 77.0 (Fahrenheit)
```

### Deep Nesting

Handle complex, deeply nested object graphs:

```csharp
public class Company
{
    public string Name { get; set; }
    public Department MainDepartment { get; set; }
    public List<Department> AllDepartments { get; set; }
}

public class Department
{
    public string Name { get; set; }
    public Manager Manager { get; set; }
    public List<Employee> Employees { get; set; }
}

public class Manager
{
    public string Name { get; set; }
    public int YearsExperience { get; set; }
}

public class Employee
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

// Corresponding DTOs with [MapFrom] attributes...

// All levels are automatically mapped
var company = new Company
{
    Name = "TechCorp",
    MainDepartment = new Department
    {
        Name = "Engineering",
        Manager = new Manager { Name = "Alice", YearsExperience = 10 },
        Employees = new List<Employee>
        {
            new Employee { FirstName = "Bob", LastName = "Smith" },
            new Employee { FirstName = "Carol", LastName = "Jones" }
        }
    }
};

var dto = company.ToCompanyDto();
// Entire object graph is mapped recursively
```

## Generated Code Example

For the basic User/UserDto example, Moedim.Mapper generates:

```csharp
// <auto-generated/>
#nullable enable

using TestNamespace;

namespace TestNamespace;

/// <summary>
/// Extension methods for mapping User to UserDto.
/// </summary>
public static class UserToUserDtoMappingExtensions
{
    /// <summary>
    /// Maps User to UserDto.
    /// </summary>
    public static UserDto? ToUserDto(this User? source)
    {
        if (source is null)
            return null;

        return new UserDto
        {
            Name = source.Name,
            Age = source.Age,
            Email = source.Email
        };
    }
}
```

## Performance

Moedim.Mapper uses source generators to create mapping code at compile time, resulting in:

- **Zero reflection overhead** - Direct property assignments
- **Similar to manual mapping** - Generated code is as fast as hand-written code
- **No runtime initialization** - Ready to use immediately
- **Optimized for collections** - Efficient collection transformations

Run benchmarks with:

```bash
dotnet run --project tests/Moedim.Mapper.Performance.Tests -c Release
```

## Advanced Features

### Fluent Configuration Interface

The fluent configuration API provides a programmatic way to define mappings with full IntelliSense support and type safety.

#### Basic Fluent Mapping

```csharp
using Moedim.Mapper;

var builder = new MapperConfigurationBuilder();

// Create a simple mapping
var mapping = builder.CreateMap<User, UserDto>();

// The mapping is now configured and ready to use
// Note: mapping.ForMember() returns IMappingExpression for chaining
```

#### Fluent Custom Property Mapping

Map properties with different names using the fluent API:

```csharp
var builder = new MapperConfigurationBuilder();

builder.CreateMap<User, UserDto>()
    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
    .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email));

// Custom property mappings configured:
//   - FullName ← Name
//   - EmailAddress ← Email
```

#### Fluent Ignoring Properties

Exclude specific properties from mapping:

```csharp
var builder = new MapperConfigurationBuilder();

builder.CreateMap<User, UserDto>()
    .ForMember(dest => dest.InternalId, opt => opt.Ignore())
    .ForMember(dest => dest.Metadata, opt => opt.Ignore());

// InternalId and Metadata will be excluded from mapping
```

#### Method Chaining

The fluent API supports method chaining for concise configuration:

```csharp
var builder = new MapperConfigurationBuilder();

builder.CreateMap<Customer, CustomerDto>()
    .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Name))
    .ForMember(dest => dest.PrimaryEmail, opt => opt.MapFrom(src => src.Email))
    .ForMember(dest => dest.InternalId, opt => opt.Ignore())
    .ForMember(dest => dest.DisplayAddress, opt => opt.MapFrom(src => src.Address));
```

#### Multiple Mapping Configurations

Configure multiple type mappings in a single builder:

```csharp
var builder = new MapperConfigurationBuilder();

builder.CreateMap<User, UserDto>()
    .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name));

builder.CreateMap<Customer, CustomerDto>()
    .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Name));

builder.CreateMap<Order, OrderDto>()
    .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Id));

// Multiple mappings are now configured in the builder
```

#### Fluent API Interfaces

```csharp
public interface IMapperConfigurationBuilder
{
    IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>();
}

public interface IMappingExpression<TSource, TDestination>
{
    IMappingExpression<TSource, TDestination> ForMember<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Action<IMemberConfigurationExpression<TSource, TDestination, TMember>> memberOptions);
}

public interface IMemberConfigurationExpression<TSource, TDestination, TMember>
{
    void MapFrom(Expression<Func<TSource, TMember>> sourceMember);
    void Ignore();
}
```

### Configuration Marker Attribute

Mark configuration classes for the source generator:

```csharp
[MapperConfiguration]
public class MyMappingConfiguration
{
    public void Configure(IMapperConfigurationBuilder builder)
    {
        builder.CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name));
    }
}
```

### Supported Type Conversions

- Direct type matches
- Nullable value types (int to int?, etc.)
- Collection types (Array, List, IEnumerable) with element transformation
- Numeric conversions (int to long, etc.)
- Nested complex objects (automatic recursive mapping)
- Custom value converters (IValueConverter<TSource, TDest>)

## Requirements

- .NET 6.0, 7.0, or 8.0
- C# 11 or later (for consuming projects)
- Visual Studio 2022 or Rider (for best IDE support)

## Project Structure

- **Moedim.Mapper** - Core library with attributes and interfaces
- **Moedim.Mapper.SourceGenerator** - Roslyn source generator implementation
- **Moedim.Mapper.Tests** - Unit tests
- **Moedim.Mapper.SourceGenerator.Tests** - Generator-specific tests
- **Moedim.Mapper.Performance.Tests** - Performance benchmarks
- **Moedim.Mapper.Sample** - Example usage

## Migrating from AutoMapper

Moedim.Mapper provides both automated and manual migration options:

### Automated Migration Tool

Install the migration tool globally:

```bash
dotnet tool install -g Moedim.Mapper.Migration.Tool
```

Analyze your project for AutoMapper usage:

```bash
moedim-mapper-migrate analyze path/to/YourProject.csproj --verbose
```

Migrate automatically to Moedim.Mapper:

```bash
moedim-mapper-migrate migrate path/to/YourProject.csproj
```

Generate a compatibility report:

```bash
moedim-mapper-migrate report path/to/YourProject.csproj --format html
```

For detailed tool documentation, see [Migration Tool README](tools/Moedim.Mapper.Migration.Tool/README.md).

### Manual Migration

For step-by-step manual migration guidance with code comparisons, see [MIGRATION_GUIDE.md](MIGRATION_GUIDE.md).

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

Built with:

- [Roslyn Source Generators](https://github.com/dotnet/roslyn)
- [xUnit](https://xunit.net/)
- [BenchmarkDotNet](https://benchmarkdotnet.org/)
- [FluentAssertions](https://fluentassertions.com/)

## Version History

### 1.0.0 (Initial Release)

- Attribute-based mapping with `[MapFrom]` and `[MapTo]`
- Custom property mapping with `[MapProperty]`
- Property exclusion with `[IgnoreProperty]`
- Collection mapping support with automatic item transformation
- **Nested object mapping** - Automatic recursive mapping of complex objects
- **Value converters** - Custom transformation with `[ConvertWith]` and `IValueConverter<TSource, TDest>`
- **Deep object graph mapping** - Handle multi-level nested structures
- **Collection of complex objects** - Map `List<SourceType>` to `List<DestType>`
- Null-safe code generation
- Multi-framework support (.NET 6.0, 7.0, 8.0)
