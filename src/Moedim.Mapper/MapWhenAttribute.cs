namespace Moedim.Mapper;

/// <summary>
/// Specifies a condition that must be met for a property to be mapped.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MapWhenAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapWhenAttribute"/> class.
    /// </summary>
    /// <param name="sourcePropertyName">The name of the boolean property in the source object that determines if mapping should occur.</param>
    public MapWhenAttribute(string sourcePropertyName)
    {
        if (string.IsNullOrWhiteSpace(sourcePropertyName))
        {
            throw new ArgumentException("Source property name cannot be null or whitespace.", nameof(sourcePropertyName));
        }

        SourcePropertyName = sourcePropertyName;
    }

    /// <summary>
    /// Gets the name of the source property that contains the condition.
    /// </summary>
    public string SourcePropertyName { get; }
}
