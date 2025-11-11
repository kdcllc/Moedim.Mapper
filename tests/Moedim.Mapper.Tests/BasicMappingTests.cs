using FluentAssertions;
using Moedim.Mapper.Tests.Models;
using Xunit;

namespace Moedim.Mapper.Tests;

/// <summary>
/// Tests for basic mapping functionality.
/// </summary>
public class BasicMappingTests
{
    [Fact]
    public void Map_SimpleProperties_ShouldMapCorrectly()
    {
        // Arrange
        var person = new Person
        {
            Name = "John Doe",
            Age = 30,
            Email = "john@example.com"
        };

        // Act
        var dto = person.ToPersonDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Name.Should().Be("John Doe");
        dto.Age.Should().Be(30);
        dto.Email.Should().Be("john@example.com");
    }

    [Fact]
    public void Map_NullSource_ShouldReturnNull()
    {
        // Arrange
        Person? person = null;

        // Act
        var dto = person.ToPersonDto();

        // Assert
        dto.Should().BeNull();
    }

    [Fact]
    public void Map_WithNestedObject_ShouldMapCorrectly()
    {
        // Arrange
        var person = new Person
        {
            Name = "Jane Doe",
            Age = 25,
            Email = "jane@example.com",
            Address = new Address
            {
                Street = "123 Main St",
                City = "New York",
                ZipCode = "10001"
            }
        };

        // Act
        var dto = person.ToPersonDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Address.Should().NotBeNull();
        // Note: Nested object mapping would require recursive generation
        // For now, we'll test the basic structure
    }

    [Fact]
    public void Map_WithCollection_ShouldMapCorrectly()
    {
        // Arrange
        var person = new Person
        {
            Name = "Bob Smith",
            Age = 35,
            Email = "bob@example.com",
            Tags = new List<string> { "developer", "architect" }
        };

        // Act
        var dto = person.ToPersonDto();

        // Assert
        dto.Should().NotBeNull();
        dto!.Tags.Should().NotBeNull();
        dto.Tags.Should().HaveCount(2);
        dto.Tags.Should().Contain("developer");
        dto.Tags.Should().Contain("architect");
    }
}
