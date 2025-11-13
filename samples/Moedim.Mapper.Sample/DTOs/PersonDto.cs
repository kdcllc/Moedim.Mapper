using Moedim.Mapper;
using Moedim.Mapper.Sample.Models;

namespace Moedim.Mapper.Sample.DTOs;

/// <summary>
/// Data transfer object for Person with nested contact information.
/// </summary>
[MapFrom(typeof(Person))]
public class PersonDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int Age { get; set; }
    public ContactInfoDto? Contact { get; set; }

    public string? FullName => $"{FirstName} {LastName}";
}
