namespace Moedim.Mapper;

/// <summary>
/// Defines a hook that is called before mapping occurs.
/// </summary>
/// <typeparam name="TSource">The source type.</typeparam>
/// <typeparam name="TDestination">The destination type.</typeparam>
public interface IBeforeMap<in TSource, in TDestination>
{
    /// <summary>
    /// Called before the mapping operation.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <param name="destination">The destination object being created.</param>
    void BeforeMap(TSource source, TDestination destination);
}

/// <summary>
/// Defines a hook that is called after mapping occurs.
/// </summary>
/// <typeparam name="TSource">The source type.</typeparam>
/// <typeparam name="TDestination">The destination type.</typeparam>
public interface IAfterMap<in TSource, in TDestination>
{
    /// <summary>
    /// Called after the mapping operation.
    /// </summary>
    /// <param name="source">The source object.</param>
    /// <param name="destination">The destination object that was created.</param>
    void AfterMap(TSource source, TDestination destination);
}
