namespace Moedim.Mapper.Sample.Models;

/// <summary>
/// Represents a temperature reading in Celsius.
/// </summary>
public class TemperatureReading
{
    public string? SensorId { get; set; }
    public double Celsius { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Location { get; set; }
}
