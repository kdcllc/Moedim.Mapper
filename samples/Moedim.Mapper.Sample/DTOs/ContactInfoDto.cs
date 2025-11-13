using Moedim.Mapper;
using Moedim.Mapper.Sample.Models;

namespace Moedim.Mapper.Sample.DTOs;

/// <summary>
/// Data transfer object for ContactInfo with nested addresses.
/// </summary>
[MapFrom(typeof(ContactInfo))]
public class ContactInfoDto
{
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public AddressDto? HomeAddress { get; set; }
    public AddressDto? WorkAddress { get; set; }
}
