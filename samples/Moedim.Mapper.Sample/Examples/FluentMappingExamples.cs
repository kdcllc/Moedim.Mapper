using System;
using System.Linq;

namespace Moedim.Mapper.Sample.Examples;

/// <summary>
/// Demonstrates the fluent configuration API for defining object mappings.
/// </summary>
public static class FluentMappingExamples
{
    public static void Run()
    {
        Console.WriteLine("\n=== Fluent Configuration API Examples ===\n");

        Example1_SimpleMapping();
        Example2_CustomPropertyMapping();
        Example3_IgnoreProperties();
        Example4_ComplexMapping();
        Example5_MultipleConfigurations();
    }

    private static void Example1_SimpleMapping()
    {
        Console.WriteLine("1. Simple Fluent Mapping:");

        var builder = new MapperConfigurationBuilder();

        // Create a mapping configuration
        var mapping = builder.CreateMap<UserEntity, UserDto>();

        Console.WriteLine($"   ✓ Mapping configured: UserEntity → UserDto");
        Console.WriteLine("   Configuration created successfully");
        Console.WriteLine();
    }

    private static void Example2_CustomPropertyMapping()
    {
        Console.WriteLine("2. Custom Property Mapping:");

        var builder = new MapperConfigurationBuilder();

        // Map properties with different names
        builder.CreateMap<UserEntity, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email));

        Console.WriteLine("   ✓ Custom property mappings configured:");
        Console.WriteLine("     - FullName ← Name");
        Console.WriteLine("     - EmailAddress ← Email");
        Console.WriteLine();
    }

    private static void Example3_IgnoreProperties()
    {
        Console.WriteLine("3. Ignore Properties:");

        var builder = new MapperConfigurationBuilder();

        // Ignore specific properties during mapping
        builder.CreateMap<UserEntity, UserDto>()
            .ForMember(dest => dest.IgnoreMe, opt => opt.Ignore())
            .ForMember(dest => dest.AlsoIgnore, opt => opt.Ignore());

        Console.WriteLine("   ✓ Properties configured to ignore:");
        Console.WriteLine("     - IgnoreMe");
        Console.WriteLine("     - AlsoIgnore");
        Console.WriteLine();
    }

    private static void Example4_ComplexMapping()
    {
        Console.WriteLine("4. Complex Fluent Mapping:");

        var builder = new MapperConfigurationBuilder();

        // Complex mapping with multiple customizations
        builder.CreateMap<CustomerEntity, CustomerDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.PrimaryEmail, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.InternalId, opt => opt.Ignore())
            .ForMember(dest => dest.DisplayAddress, opt => opt.MapFrom(src => src.Address));

        Console.WriteLine("   ✓ Complex mapping: CustomerEntity → CustomerDto");
        Console.WriteLine("   Custom mappings:");
        Console.WriteLine("     - CustomerName ← Name");
        Console.WriteLine("     - PrimaryEmail ← Email");
        Console.WriteLine("     - DisplayAddress ← Address");
        Console.WriteLine("   Ignored:");
        Console.WriteLine("     - InternalId");
        Console.WriteLine();
    }

    private static void Example5_MultipleConfigurations()
    {
        Console.WriteLine("5. Multiple Mapping Configurations:");

        var builder = new MapperConfigurationBuilder();

        // Configure multiple mappings
        builder.CreateMap<UserEntity, UserDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.Name));

        builder.CreateMap<CustomerEntity, CustomerDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Name));

        builder.CreateMap<OrderEntity, OrderDto>()
            .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.Id));

        Console.WriteLine("   ✓ Multiple mappings configured:");
        Console.WriteLine("     - UserEntity → UserDto");
        Console.WriteLine("     - CustomerEntity → CustomerDto");
        Console.WriteLine("     - OrderEntity → OrderDto");
        Console.WriteLine();
    }

    // Example Models for Fluent Configuration
    public class UserEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class UserDto
    {
        public string FullName { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public int Age { get; set; }
        public string IgnoreMe { get; set; } = string.Empty;
        public string AlsoIgnore { get; set; } = string.Empty;
    }

    public class CustomerEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

    public class CustomerDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string PrimaryEmail { get; set; } = string.Empty;
        public string DisplayAddress { get; set; } = string.Empty;
        public string InternalId { get; set; } = string.Empty;
    }

    public class OrderEntity
    {
        public string Id { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }

    public class OrderDto
    {
        public string OrderNumber { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}
