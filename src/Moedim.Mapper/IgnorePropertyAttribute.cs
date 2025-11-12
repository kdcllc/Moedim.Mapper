namespace Moedim.Mapper;

/// <summary>
/// Indicates that a property should be ignored during mapping.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class IgnorePropertyAttribute : Attribute
{
}
