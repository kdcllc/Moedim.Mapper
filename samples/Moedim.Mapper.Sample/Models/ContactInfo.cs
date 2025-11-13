namespace Moedim.Mapper.Sample.Models;

/// <summary>
/// Contact information for a person.
/// </summary>
public class ContactInfo
{
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Address? HomeAddress { get; set; }
    public Address? WorkAddress { get; set; }
}
