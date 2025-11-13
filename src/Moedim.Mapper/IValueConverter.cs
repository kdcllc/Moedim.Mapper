namespace Moedim.Mapper;

/// <summary>
/// Defines a custom value converter for transforming values during mapping.
/// </summary>
/// <typeparam name="TSource">The source value type.</typeparam>
/// <typeparam name="TDestination">The destination value type.</typeparam>
public interface IValueConverter<in TSource, out TDestination>
{
    /// <summary>
    /// Converts a source value to a destination value.
    /// </summary>
    /// <param name="source">The source value.</param>
    /// <returns>The converted destination value.</returns>
    TDestination Convert(TSource source);
}
