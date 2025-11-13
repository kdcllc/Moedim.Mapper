namespace Moedim.Mapper;

/// <summary>
/// Specifies a custom property mapping from a source property with a different name.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MapPropertyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapPropertyAttribute"/> class.
    /// </summary>
    /// <param name="sourcePropertyName">The name of the source property to map from.</param>
    public MapPropertyAttribute(string sourcePropertyName)
    {
        if (string.IsNullOrWhiteSpace(sourcePropertyName))
        {
            throw new ArgumentException("Source property name cannot be null or whitespace.", nameof(sourcePropertyName));
        }

        SourcePropertyName = sourcePropertyName;
    }

    /// <summary>
    /// Gets the name of the source property to map from.
    /// </summary>
    public string SourcePropertyName { get; }
}
