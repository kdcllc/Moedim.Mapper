namespace Moedim.Mapper;

/// <summary>
/// Indicates that the source class should have mapping methods generated to the specified destination type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class MapToAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapToAttribute"/> class.
    /// </summary>
    /// <param name="destinationType">The destination type to map to.</param>
    public MapToAttribute(Type destinationType)
    {
        DestinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
    }

    /// <summary>
    /// Gets the destination type to map to.
    /// </summary>
    public Type DestinationType { get; }
}
