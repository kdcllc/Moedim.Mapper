using System;

namespace Moedim.Mapper;

/// <summary>
/// Internal attribute used by the source generator to mark types configured via fluent API.
/// This attribute is applied automatically and should not be used directly.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal sealed class FluentMappingAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FluentMappingAttribute"/> class.
    /// </summary>
    /// <param name="sourceMemberName">The name of the source member to map from.</param>
    public FluentMappingAttribute(string sourceMemberName)
    {
        SourceMemberName = sourceMemberName;
    }

    /// <summary>
    /// Gets the name of the source member to map from.
    /// </summary>
    public string SourceMemberName { get; }
}
