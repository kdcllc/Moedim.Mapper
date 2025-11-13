using Moedim.Mapper;

namespace Moedim.Mapper.Sample.Converters;

/// <summary>
/// Converts temperature from Celsius to Fahrenheit.
/// </summary>
public class CelsiusToFahrenheitConverter : IValueConverter<double, double>
{
    public double Convert(double celsius)
    {
        return (celsius * 9.0 / 5.0) + 32.0;
    }
}
