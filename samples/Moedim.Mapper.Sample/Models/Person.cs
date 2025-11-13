namespace Moedim.Mapper.Sample.Models;

/// <summary>
/// Represents a person with nested contact information.
/// </summary>
public class Person
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
    public ContactInfo? Contact { get; set; }
}
