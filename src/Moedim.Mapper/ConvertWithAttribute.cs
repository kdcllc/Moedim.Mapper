namespace Moedim.Mapper;

/// <summary>
/// Specifies a custom value converter to use when mapping a property.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ConvertWithAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConvertWithAttribute"/> class.
    /// </summary>
    /// <param name="converterType">The type of converter that implements IValueConverter.</param>
    public ConvertWithAttribute(Type converterType)
    {
        ConverterType = converterType ?? throw new ArgumentNullException(nameof(converterType));
    }

    /// <summary>
    /// Gets the type of converter to use.
    /// </summary>
    public Type ConverterType { get; }
}
