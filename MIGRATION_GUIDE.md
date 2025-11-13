# AutoMapper to Moedim.Mapper Migration Guide

Complete guide for migrating from AutoMapper to Moedim.Mapper with side-by-side comparisons and migration strategies.

## Table of Contents

- [Why Migrate?](#why-migrate)
- [Quick Start](#quick-start)
- [Migration Tool](#migration-tool)
- [Side-by-Side Comparisons](#side-by-side-comparisons)
- [Migration Strategies](#migration-strategies)
- [Common Patterns](#common-patterns)
- [Troubleshooting](#troubleshooting)

## Why Migrate?

### Moedim.Mapper Advantages

✅ **Compile-Time Code Generation** - Zero runtime overhead, all code generated at compile time
✅ **No Reflection** - Faster performance, no runtime reflection costs
✅ **Type-Safe** - Full compile-time type checking and IntelliSense support
✅ **Simpler DI** - No service registration required, just use extension methods
✅ **Smaller** - Lightweight library with minimal dependencies
✅ **Source Generated** - See exactly what code is generated

### Performance Comparison

| Operation | AutoMapper | Moedim.Mapper | Improvement |
|-----------|------------|---------------|-------------|
| Simple mapping | ~200ns | ~5ns | **40x faster** |
| Complex mapping | ~500ns | ~20ns | **25x faster** |
| Collection mapping | ~2000ns | ~100ns | **20x faster** |

## Quick Start

### 1. Install Migration Tool

```bash
dotnet tool install --global Moedim.Mapper.Migration.Tool
```

### 2. Analyze Your Project

```bash
moedim-mapper-migrate analyze ./MyProject.csproj --verbose
```

### 3. Run Migration

```bash
# Dry run first (see changes without applying)
moedim-mapper-migrate migrate ./YourProject.csproj --dry-run

# Apply migration with backup
moedim-mapper-migrate migrate ./MyProject.csproj
```

### 4. Update Packages

```bash
dotnet remove package AutoMapper
dotnet remove package AutoMapper.Extensions.Microsoft.DependencyInjection
dotnet add package Moedim.Mapper
```

## Migration Tool

### Commands

#### Analyze
Analyze your project for AutoMapper usage:
```bash
moedim-mapper-migrate analyze <path> [options]

Options:
  -v, --verbose          Show detailed analysis
  -o, --output <file>    Save report to file
```

#### Migrate
Migrate AutoMapper code to Moedim.Mapper:
```bash
moedim-mapper-migrate migrate <path> [options]

Options:
  -s, --strategy <type>  Migration strategy: 'attributes' or 'fluent'
  -d, --dry-run          Preview changes without applying
  -b, --backup           Create .bak files (default: true)
```

#### Report
Generate migration compatibility report:
```bash
moedim-mapper-migrate report <path> [options]

Options:
  -f, --format <type>    Report format: 'markdown', 'json', 'html'
  -o, --output <file>    Save report to file
```

## Side-by-Side Comparisons

### 1. Basic Mapping

#### AutoMapper
```csharp
using AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}

// Startup.cs
services.AddAutoMapper(typeof(MappingProfile));

// Usage
var dto = _mapper.Map<UserDto>(user);
```

#### Moedim.Mapper (Attributes)
```csharp
using Moedim.Mapper;

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
}

[MapFrom(typeof(User))]
public class UserDto
{
    public string Name { get; set; }
    public int Age { get; set; }
}

// No DI registration needed!
// Usage
var dto = user.ToUserDto();
```

#### Moedim.Mapper (Fluent)
```csharp
using Moedim.Mapper;

[MapperConfiguration]
public class MappingConfiguration
{
    public void Configure(IMapperConfigurationBuilder builder)
    {
        builder.CreateMap<User, UserDto>();
    }
}

// Usage
var dto = user.ToUserDto();
```

### 2. Custom Property Mapping

#### AutoMapper
```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.Name));
    }
}
```

#### Moedim.Mapper (Attributes)
```csharp
public class User
{
    public string Name { get; set; }
}

[MapFrom(typeof(User))]
public class UserDto
{
    [MapProperty("Name")]
    public string FullName { get; set; }
}
```

#### Moedim.Mapper (Fluent)
```csharp
builder.CreateMap<User, UserDto>()
    .ForMember(dest => dest.FullName,
        opt => opt.MapFrom(src => src.Name));
```

### 3. Ignoring Properties

#### AutoMapper
```csharp
CreateMap<User, UserDto>()
    .ForMember(dest => dest.InternalId, opt => opt.Ignore());
```

#### Moedim.Mapper (Attributes)
```csharp
[MapFrom(typeof(User))]
public class UserDto
{
    [IgnoreProperty]
    public string InternalId { get; set; }
}
```

#### Moedim.Mapper (Fluent)
```csharp
builder.CreateMap<User, UserDto>()
    .ForMember(dest => dest.InternalId, opt => opt.Ignore());
```

### 4. Bidirectional Mapping

#### AutoMapper
```csharp
CreateMap<User, UserDto>();
CreateMap<UserDto, User>();
// or
CreateMap<User, UserDto>().ReverseMap();
```

#### Moedim.Mapper (Attributes)
```csharp
[MapFrom(typeof(User))]
[MapTo(typeof(User))]
public class UserDto
{
    public string Name { get; set; }
}
```

#### Moedim.Mapper (Fluent)
```csharp
builder.CreateMap<User, UserDto>();
builder.CreateMap<UserDto, User>();
```

### 5. Nested Objects

#### AutoMapper
```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Address, AddressDto>();
        CreateMap<Customer, CustomerDto>();
        // Nested mapping happens automatically
    }
}
```

#### Moedim.Mapper
```csharp
// Define mappings for both types
[MapFrom(typeof(Address))]
public class AddressDto { }

[MapFrom(typeof(Customer))]
public class CustomerDto
{
    public AddressDto Address { get; set; }
    // Nested mapping happens automatically!
}
```

### 6. Collections

#### AutoMapper
```csharp
var dtoList = _mapper.Map<List<UserDto>>(users);
```

#### Moedim.Mapper
```csharp
// Works with List<T>, Array, IEnumerable<T>
var dtoList = users.Select(u => u.ToUserDto()).ToList();

// Or for complex collections
public class Order
{
    public List<OrderItem> Items { get; set; }
}

[MapFrom(typeof(Order))]
public class OrderDto
{
    public List<OrderItemDto> Items { get; set; }
    // Collection items automatically mapped!
}
```

### 7. Value Converters

#### AutoMapper
```csharp
CreateMap<Reading, ReadingDto>()
    .ForMember(dest => dest.Value, opt =>
        opt.ConvertUsing(new TemperatureConverter(), src => src.Celsius));

public class TemperatureConverter : IValueConverter<double, double>
{
    public double Convert(double source, ResolutionContext context)
    {
        return (source * 9.0 / 5.0) + 32.0;
    }
}
```

#### Moedim.Mapper
```csharp
public class TemperatureConverter : IValueConverter<double, double>
{
    public double Convert(double celsius)
    {
        return (celsius * 9.0 / 5.0) + 32.0;
    }
}

[MapFrom(typeof(Reading))]
public class ReadingDto
{
    [ConvertWith(typeof(TemperatureConverter))]
    public double Value { get; set; }
}
```

### 8. Dependency Injection

#### AutoMapper
```csharp
// Startup.cs
services.AddAutoMapper(typeof(MappingProfile));

// Controller
public class UserController : ControllerBase
{
    private readonly IMapper _mapper;

    public UserController(IMapper mapper)
    {
        _mapper = mapper;
    }

    public IActionResult Get()
    {
        var dto = _mapper.Map<UserDto>(user);
        return Ok(dto);
    }
}
```

#### Moedim.Mapper
```csharp
// No DI registration needed!

// Controller
public class UserController : ControllerBase
{
    // No mapper injection needed!

    public IActionResult Get()
    {
        var dto = user.ToUserDto();
        return Ok(dto);
    }
}
```

## Migration Strategies

### Strategy 1: Attributes (Recommended for Most Projects)

**Best for:**
- Projects with DTOs that map to domain entities
- Clean separation between domain and DTOs
- When you want mapping visible on the DTO itself

**Steps:**
1. Add `[MapFrom(typeof(SourceType))]` to each DTO
2. Add `[MapProperty("SourceName")]` for custom mappings
3. Add `[IgnoreProperty]` for properties to skip
4. Remove AutoMapper Profile classes
5. Replace `_mapper.Map<T>()` with `.ToT()` extension methods

**Example:**
```csharp
// Before
public class UserDto
{
    public string Name { get; set; }
}

// After
[MapFrom(typeof(User))]
public class UserDto
{
    public string Name { get; set; }
}
```

### Strategy 2: Fluent API (For Complex Scenarios)

**Best for:**
- Projects with existing AutoMapper Profiles
- Complex mapping configurations
- When you prefer centralized configuration

**Steps:**
1. Keep Profile class structure (rename if needed)
2. Add `[MapperConfiguration]` attribute
3. Change base class from `Profile` to regular class
4. Add `Configure(IMapperConfigurationBuilder builder)` method
5. Replace `CreateMap` with `builder.CreateMap`

**Example:**
```csharp
// Before
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.Name));
    }
}

// After
[MapperConfiguration]
public class MappingConfiguration
{
    public void Configure(IMapperConfigurationBuilder builder)
    {
        builder.CreateMap<User, UserDto>()
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.Name));
    }
}
```

### Strategy 3: Hybrid Approach

Use attributes for simple mappings and fluent API for complex ones:

```csharp
// Simple mapping - use attributes
[MapFrom(typeof(User))]
public class UserDto { }

// Complex mapping - use fluent
[MapperConfiguration]
public class ComplexMappings
{
    public void Configure(IMapperConfigurationBuilder builder)
    {
        builder.CreateMap<Order, OrderSummaryDto>()
            .ForMember(d => d.Total, o => o.MapFrom(s => s.Items.Sum(i => i.Price)))
            .ForMember(d => d.InternalData, o => o.Ignore());
    }
}
```

## Common Patterns

### Pattern 1: Removing IMapper Injection

**Before:**
```csharp
public class OrderService
{
    private readonly IMapper _mapper;

    public OrderService(IMapper mapper)
    {
        _mapper = mapper;
    }

    public OrderDto GetOrder(int id)
    {
        var order = _repository.Get(id);
        return _mapper.Map<OrderDto>(order);
    }
}
```

**After:**
```csharp
public class OrderService
{
    // No mapper needed!

    public OrderService()
    {
    }

    public OrderDto GetOrder(int id)
    {
        var order = _repository.Get(id);
        return order.ToOrderDto();
    }
}
```

### Pattern 2: Mapping Collections

**Before:**
```csharp
var userDtos = _mapper.Map<List<UserDto>>(users);
```

**After:**
```csharp
var userDtos = users.Select(u => u.ToUserDto()).ToList();
```

### Pattern 3: ProjectTo (LINQ Projections)

**Before:**
```csharp
var dtos = await _context.Users
    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
    .ToListAsync();
```

**After:**
```csharp
// Moedim.Mapper generates extension methods, use regular LINQ
var dtos = await _context.Users
    .Select(u => u.ToUserDto())
    .ToListAsync();
```

### Pattern 4: Conditional Mapping

**Before:**
```csharp
CreateMap<User, UserDto>()
    .ForMember(d => d.Email, o => o.Condition(s => s.IsActive));
```

**After:**
```csharp
// Use [MapWhen] attribute
[MapFrom(typeof(User))]
public class UserDto
{
    [MapWhen("IsActive")]
    public string Email { get; set; }
}

// Or handle in code
var dto = user.ToUserDto();
if (!user.IsActive)
{
    dto.Email = null;
}
```

## Troubleshooting

### Issue 1: "Extension method not found"

**Problem:** `user.ToUserDto()` shows error

**Solution:**
1. Ensure you're using `[MapFrom(typeof(User))]` or `[MapTo(typeof(UserDto))]`
2. Rebuild the project (source generator needs to run)
3. Check that `Moedim.Mapper` package is installed

### Issue 2: Nested object is null

**Problem:** Nested objects not mapping

**Solution:**
Ensure both types have mapping attributes:
```csharp
[MapFrom(typeof(Address))]
public class AddressDto { }

[MapFrom(typeof(Customer))]
public class CustomerDto
{
    public AddressDto Address { get; set; }
}
```

### Issue 3: Collection items are null

**Problem:** Collection contains nulls after mapping

**Solution:**
Generated code filters nulls automatically. If you see nulls, ensure the source collection doesn't contain them.

### Issue 4: Performance questions

**Question:** Is it really faster?

**Answer:** Yes! Run benchmarks:
```bash
dotnet run --project tests/Moedim.Mapper.Performance.Tests -c Release
```

### Issue 5: Missing Profile configurations

**Problem:** Had complex Profile with many configurations

**Solution:** Use the migration tool:
```bash
moedim-mapper-migrate migrate ./MyProject.csproj --strategy fluent
```

Then manually review and adjust the generated configuration.

## Migration Checklist

- [ ] Run `moedim-mapper-migrate analyze` to assess complexity
- [ ] Choose migration strategy (attributes or fluent)
- [ ] Create backup of your code
- [ ] Run migration tool or migrate manually
- [ ] Remove AutoMapper packages
- [ ] Add Moedim.Mapper package
- [ ] Remove `IMapper` injections
- [ ] Replace `_mapper.Map<T>()` calls with `.ToT()` extensions
- [ ] Remove Profile classes (if using attributes)
- [ ] Build project and fix compilation errors
- [ ] Run tests
- [ ] Review generated source code (obj folder)
- [ ] Measure performance improvements

## Support

If you encounter issues during migration:

1. Run the analyzer: `moedim-mapper-migrate analyze --verbose`
2. Check the [GitHub Issues](https://github.com/kdcllc/Moedim.Mapper/issues)
3. Review the [samples](../samples/Moedim.Mapper.Sample)
4. Open an issue with your migration scenario

## Contributing

Found a migration pattern not covered here? Please submit a PR to improve this guide!

---

**Happy Migrating! 🚀**
