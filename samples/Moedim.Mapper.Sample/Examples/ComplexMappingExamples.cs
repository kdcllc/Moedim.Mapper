using Moedim.Mapper.Sample.Models;
using Moedim.Mapper.Sample.DTOs;

namespace Moedim.Mapper.Sample.Examples;

/// <summary>
/// Demonstrates complex object mapping scenarios.
/// </summary>
public static class ComplexMappingExamples
{
    public static void Run()
    {
        Console.WriteLine("=== Complex Object Mapping Examples ===\n");

        // Example 1: Nested Object Mapping
        RunNestedObjectExample();

        // Example 2: Collections of Complex Objects
        RunCollectionMappingExample();

        // Example 3: Deep Nesting
        RunDeepNestingExample();

        // Example 4: Value Converter
        RunValueConverterExample();
    }

    private static void RunNestedObjectExample()
    {
        Console.WriteLine("1. Nested Object Mapping:");

        var person = new Person
        {
            FirstName = "Alice",
            LastName = "Johnson",
            Age = 28,
            Contact = new ContactInfo
            {
                Email = "alice@example.com",
                Phone = "+1-555-0123",
                HomeAddress = new Address
                {
                    Street = "123 Main St",
                    City = "Seattle",
                    State = "WA",
                    ZipCode = "98101",
                    Country = "USA"
                }
            }
        };

        var personDto = person.ToPersonDto();

        Console.WriteLine($"   Person: {personDto?.FullName}, Age: {personDto?.Age}");
        Console.WriteLine($"   Email: {personDto?.Contact?.Email}");
        Console.WriteLine($"   Address: {personDto?.Contact?.HomeAddress?.Street}, {personDto?.Contact?.HomeAddress?.City}");
        Console.WriteLine();
    }

    private static void RunCollectionMappingExample()
    {
        Console.WriteLine("2. Collections of Complex Objects:");

        var order = new Order
        {
            OrderId = "ORD-2025-001",
            OrderDate = DateTime.Now,
            Customer = new Person
            {
                FirstName = "Bob",
                LastName = "Smith",
                Age = 35,
                Contact = new ContactInfo
                {
                    Email = "bob@example.com",
                    Phone = "+1-555-0456"
                }
            },
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    ProductId = "PROD-001",
                    ProductName = "Laptop",
                    Quantity = 1,
                    UnitPrice = 1299.99m
                },
                new OrderItem
                {
                    ProductId = "PROD-002",
                    ProductName = "Wireless Mouse",
                    Quantity = 2,
                    UnitPrice = 29.99m
                },
                new OrderItem
                {
                    ProductId = "PROD-003",
                    ProductName = "USB-C Cable",
                    Quantity = 3,
                    UnitPrice = 12.99m
                }
            },
            TotalAmount = 1398.94m,
            Status = "Processing"
        };

        var orderDto = order.ToOrderDto();

        Console.WriteLine($"   Order ID: {orderDto?.OrderId}");
        Console.WriteLine($"   Customer: {orderDto?.Customer?.FullName}");
        Console.WriteLine($"   Items ({orderDto?.ItemCount}):");

        if (orderDto?.Items != null)
        {
            foreach (var item in orderDto.Items)
            {
                Console.WriteLine($"     - {item.ProductName}: {item.Quantity} x ${item.UnitPrice:F2} = ${item.TotalPrice:F2}");
            }
        }

        Console.WriteLine($"   Total: ${orderDto?.TotalAmount:F2}");
        Console.WriteLine();
    }

    private static void RunDeepNestingExample()
    {
        Console.WriteLine("3. Deep Nesting with Multiple Addresses:");

        var businessPerson = new Person
        {
            FirstName = "Carol",
            LastName = "Williams",
            Age = 42,
            Contact = new ContactInfo
            {
                Email = "carol@business.com",
                Phone = "+1-555-0789",
                HomeAddress = new Address
                {
                    Street = "456 Oak Avenue",
                    City = "Portland",
                    State = "OR",
                    ZipCode = "97201",
                    Country = "USA"
                },
                WorkAddress = new Address
                {
                    Street = "789 Corporate Blvd, Suite 500",
                    City = "Portland",
                    State = "OR",
                    ZipCode = "97204",
                    Country = "USA"
                }
            }
        };

        var dto = businessPerson.ToPersonDto();

        Console.WriteLine($"   Person: {dto?.FullName}");
        Console.WriteLine($"   Home: {dto?.Contact?.HomeAddress?.Street}, {dto?.Contact?.HomeAddress?.City}");
        Console.WriteLine($"   Work: {dto?.Contact?.WorkAddress?.Street}, {dto?.Contact?.WorkAddress?.City}");
        Console.WriteLine();
    }

    private static void RunValueConverterExample()
    {
        Console.WriteLine("4. Value Converter (Celsius to Fahrenheit):");

        var readings = new List<TemperatureReading>
        {
            new TemperatureReading
            {
                SensorId = "SENSOR-001",
                Celsius = 0,
                Timestamp = DateTime.Now.AddHours(-2),
                Location = "Freezer"
            },
            new TemperatureReading
            {
                SensorId = "SENSOR-002",
                Celsius = 25,
                Timestamp = DateTime.Now.AddHours(-1),
                Location = "Office"
            },
            new TemperatureReading
            {
                SensorId = "SENSOR-003",
                Celsius = 100,
                Timestamp = DateTime.Now,
                Location = "Boiler Room"
            }
        };

        Console.WriteLine("   Temperature Readings (converted to Fahrenheit):");
        foreach (var reading in readings)
        {
            var dto = reading.ToTemperatureReadingDto();
            Console.WriteLine($"     {dto?.Location}: {dto?.Celsius:F1}°F (was {reading.Celsius}°C) at {dto?.Timestamp:HH:mm}");
        }
        Console.WriteLine();
    }
}
