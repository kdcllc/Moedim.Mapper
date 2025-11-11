namespace Moedim.Mapper;

/// <summary>
/// Indicates that the target class should have mapping methods generated from the specified source type.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class MapFromAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapFromAttribute"/> class.
    /// </summary>
    /// <param name="sourceType">The source type to map from.</param>
    public MapFromAttribute(Type sourceType)
    {
        SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
    }

    /// <summary>
    /// Gets the source type to map from.
    /// </summary>
    public Type SourceType { get; }
}
