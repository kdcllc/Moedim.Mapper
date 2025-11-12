namespace Moedim.Mapper.Tests.Models;

/// <summary>
/// Source model for testing.
/// </summary>
public class Person
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
    public Address? Address { get; set; }
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Destination model for testing.
/// </summary>
[MapFrom(typeof(Person))]
public class PersonDto
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
    public AddressDto? Address { get; set; }
    public List<string>? Tags { get; set; }
}

/// <summary>
/// Address source model.
/// </summary>
public class Address
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }
}

/// <summary>
/// Address DTO.
/// </summary>
[MapFrom(typeof(Address))]
public class AddressDto
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? ZipCode { get; set; }
}
