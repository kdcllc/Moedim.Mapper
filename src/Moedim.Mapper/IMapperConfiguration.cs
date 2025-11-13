using System.Linq.Expressions;

namespace Moedim.Mapper;

/// <summary>
/// Provides a fluent interface for configuring object mappings.
/// </summary>
public interface IMapperConfigurationBuilder
{
    /// <summary>
    /// Creates a mapping configuration between a source and destination type.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <returns>A mapping expression builder.</returns>
    IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>();
}

/// <summary>
/// Provides a fluent interface for configuring a specific type mapping.
/// </summary>
/// <typeparam name="TSource">The source type.</typeparam>
/// <typeparam name="TDestination">The destination type.</typeparam>
public interface IMappingExpression<TSource, TDestination>
{
    /// <summary>
    /// Configures a custom mapping for a destination member.
    /// </summary>
    /// <typeparam name="TMember">The member type.</typeparam>
    /// <param name="destinationMember">Expression to select the destination member.</param>
    /// <param name="memberOptions">Configuration options for the member.</param>
    /// <returns>The mapping expression for method chaining.</returns>
    IMappingExpression<TSource, TDestination> ForMember<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Action<IMemberConfigurationExpression<TSource, TDestination, TMember>> memberOptions);
}

/// <summary>
/// Provides configuration options for a member mapping.
/// </summary>
/// <typeparam name="TSource">The source type.</typeparam>
/// <typeparam name="TDestination">The destination type.</typeparam>
/// <typeparam name="TMember">The member type.</typeparam>
public interface IMemberConfigurationExpression<TSource, TDestination, TMember>
{
    /// <summary>
    /// Specifies the source member or expression to map from.
    /// </summary>
    /// <param name="sourceMember">Expression to select the source value.</param>
    void MapFrom(Expression<Func<TSource, TMember>> sourceMember);

    /// <summary>
    /// Ignores this member during mapping.
    /// </summary>
    void Ignore();
}
