using Moedim.Mapper;
using Moedim.Mapper.Sample.Models;

namespace Moedim.Mapper.Sample.DTOs;

/// <summary>
/// Data transfer object for Address.
/// </summary>
[MapFrom(typeof(Address))]
public class AddressDto
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
}
