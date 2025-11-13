using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Moedim.Mapper;

/// <summary>
/// Provides a fluent interface for configuring a specific type mapping.
/// </summary>
/// <typeparam name="TSource">The source type.</typeparam>
/// <typeparam name="TDestination">The destination type.</typeparam>
public sealed class MappingExpression<TSource, TDestination> : IMappingExpression<TSource, TDestination>
{
    private readonly List<MemberConfiguration> _memberConfigurations = new();

    /// <summary>
    /// Gets the source type.
    /// </summary>
    /// <remarks>
    /// This property is intended for testing purposes only and is not part of the public API.
    /// </remarks>
    internal Type SourceType => typeof(TSource);

    /// <summary>
    /// Gets the destination type.
    /// </summary>
    /// <remarks>
    /// This property is intended for testing purposes only and is not part of the public API.
    /// </remarks>
    internal Type DestinationType => typeof(TDestination);

    /// <summary>
    /// Gets all configured member mappings.
    /// </summary>
    /// <remarks>
    /// This property is intended for testing purposes only and is not part of the public API.
    /// </remarks>
    internal IReadOnlyList<MemberConfiguration> MemberConfigurations => _memberConfigurations;

    /// <summary>
    /// Configures a custom mapping for a destination member.
    /// </summary>
    /// <typeparam name="TMember">The member type.</typeparam>
    /// <param name="destinationMember">Expression to select the destination member.</param>
    /// <param name="memberOptions">Configuration options for the member.</param>
    /// <returns>The mapping expression for method chaining.</returns>
    public IMappingExpression<TSource, TDestination> ForMember<TMember>(
        Expression<Func<TDestination, TMember>> destinationMember,
        Action<IMemberConfigurationExpression<TSource, TDestination, TMember>> memberOptions)
    {
        if (destinationMember == null)
            throw new ArgumentNullException(nameof(destinationMember));
        if (memberOptions == null)
            throw new ArgumentNullException(nameof(memberOptions));

        var memberName = GetMemberName(destinationMember);
        var memberConfig = new MemberConfiguration<TSource, TDestination, TMember>(memberName);
        memberOptions(memberConfig);
        _memberConfigurations.Add(memberConfig);

        return this;
    }

    private static string GetMemberName<TMember>(Expression<Func<TDestination, TMember>> expression)
    {
        if (expression.Body is MemberExpression memberExpr)
        {
            return memberExpr.Member.Name;
        }

        throw new ArgumentException("Expression must be a member access expression", nameof(expression));
    }

    /// <summary>
    /// Base class for member configuration.
    /// </summary>
    /// <remarks>
    /// This class is intended for testing purposes only and is not part of the public API.
    /// </remarks>
    internal abstract class MemberConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberConfiguration"/> class.
        /// </summary>
        /// <param name="memberName">The name of the member.</param>
        protected MemberConfiguration(string memberName)
        {
            MemberName = memberName;
        }

        /// <summary>
        /// Gets the name of the destination member.
        /// </summary>
        public string MemberName { get; }

        /// <summary>
        /// Gets whether this member should be ignored.
        /// </summary>
        public bool IsIgnored { get; protected set; }

        /// <summary>
        /// Gets the source member name or expression.
        /// </summary>
        public string? SourceMemberName { get; protected set; }
    }
}

/// <summary>
/// Provides configuration options for a member mapping.
/// </summary>
/// <typeparam name="TSource">The source type.</typeparam>
/// <typeparam name="TDestination">The destination type.</typeparam>
/// <typeparam name="TMember">The member type.</typeparam>
internal sealed class MemberConfiguration<TSource, TDestination, TMember>
    : MappingExpression<TSource, TDestination>.MemberConfiguration,
      IMemberConfigurationExpression<TSource, TDestination, TMember>
{
    public MemberConfiguration(string memberName) : base(memberName)
    {
    }

    /// <summary>
    /// Specifies the source member or expression to map from.
    /// </summary>
    /// <param name="sourceMember">Expression to select the source value.</param>
    public void MapFrom(Expression<Func<TSource, TMember>> sourceMember)
    {
        if (sourceMember == null)
            throw new ArgumentNullException(nameof(sourceMember));

        if (sourceMember.Body is MemberExpression memberExpr)
        {
            SourceMemberName = memberExpr.Member.Name;
        }
        else
        {
            throw new ArgumentException("MapFrom only supports simple member access expressions", nameof(sourceMember));
        }
    }

    /// <summary>
    /// Ignores this member during mapping.
    /// </summary>
    public void Ignore()
    {
        IsIgnored = true;
    }
}
