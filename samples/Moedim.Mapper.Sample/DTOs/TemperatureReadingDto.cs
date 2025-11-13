using Moedim.Mapper;
using Moedim.Mapper.Sample.Models;
using Moedim.Mapper.Sample.Converters;

namespace Moedim.Mapper.Sample.DTOs;

/// <summary>
/// Data transfer object for TemperatureReading with Fahrenheit conversion.
/// </summary>
[MapFrom(typeof(TemperatureReading))]
public class TemperatureReadingDto
{
    public string? SensorId { get; set; }

    [ConvertWith(typeof(CelsiusToFahrenheitConverter))]
    public double Celsius { get; set; }  // Will contain Fahrenheit value after conversion

    public DateTime Timestamp { get; set; }
    public string? Location { get; set; }
}
