using System;
using System.Collections.Generic;

namespace Moedim.Mapper;

/// <summary>
/// Implements a fluent interface for configuring object mappings.
/// </summary>
public sealed class MapperConfigurationBuilder : IMapperConfigurationBuilder
{
    private readonly List<object> _mappingExpressions = new();

    /// <summary>
    /// Gets all configured mapping expressions.
    /// </summary>
    /// <remarks>
    /// This property is intended for testing purposes only and is not part of the public API.
    /// </remarks>
    internal IReadOnlyList<object> MappingExpressions => _mappingExpressions;

    /// <summary>
    /// Creates a mapping configuration between a source and destination type.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <returns>A mapping expression builder.</returns>
    public IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
    {
        var expression = new MappingExpression<TSource, TDestination>();
        _mappingExpressions.Add(expression);
        return expression;
    }

    /// <summary>
    /// Gets a mapping expression for specific source and destination types.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <returns>The mapping expression if found; otherwise, null.</returns>
    /// <remarks>
    /// This method is intended for testing purposes only and is not part of the public API.
    /// </remarks>
    internal MappingExpression<TSource, TDestination>? GetMappingExpression<TSource, TDestination>()
    {
        foreach (var expr in _mappingExpressions)
        {
            if (expr is MappingExpression<TSource, TDestination> typedExpr)
            {
                return typedExpr;
            }
        }
        return null;
    }
}
