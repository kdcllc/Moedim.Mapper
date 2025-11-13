using FluentAssertions;
using Xunit;
using System;
using System.Linq;

namespace Moedim.Mapper.Tests.FluentConfiguration;

public class FluentConfigurationTests
{
    [Fact]
    public void CreateMap_ShouldCreateMappingExpression()
    {
        // Arrange
        var builder = new MapperConfigurationBuilder();

        // Act
        var expression = builder.CreateMap<Source, Destination>();

        // Assert
        expression.Should().NotBeNull();
        builder.MappingExpressions.Should().HaveCount(1);
    }

    [Fact]
    public void ForMember_WithMapFrom_ShouldConfigurePropertyMapping()
    {
        // Arrange
        var builder = new MapperConfigurationBuilder();
        var expression = builder.CreateMap<Source, Destination>();

        // Act
        expression.ForMember(d => d.TargetName, opt => opt.MapFrom(s => s.SourceName));

        // Assert
        var mappingExpr = builder.GetMappingExpression<Source, Destination>();
        mappingExpr.Should().NotBeNull();
        mappingExpr!.MemberConfigurations.Should().HaveCount(1);
        mappingExpr.MemberConfigurations[0].MemberName.Should().Be("TargetName");
        mappingExpr.MemberConfigurations[0].SourceMemberName.Should().Be("SourceName");
        mappingExpr.MemberConfigurations[0].IsIgnored.Should().BeFalse();
    }

    [Fact]
    public void ForMember_WithIgnore_ShouldMarkPropertyAsIgnored()
    {
        // Arrange
        var builder = new MapperConfigurationBuilder();
        var expression = builder.CreateMap<Source, Destination>();

        // Act
        expression.ForMember(d => d.IgnoredProperty, opt => opt.Ignore());

        // Assert
        var mappingExpr = builder.GetMappingExpression<Source, Destination>();
        mappingExpr.Should().NotBeNull();
        mappingExpr!.MemberConfigurations.Should().HaveCount(1);
        mappingExpr.MemberConfigurations[0].MemberName.Should().Be("IgnoredProperty");
        mappingExpr.MemberConfigurations[0].IsIgnored.Should().BeTrue();
    }

    [Fact]
    public void ForMember_MultipleMappings_ShouldConfigureAllMembers()
    {
        // Arrange
        var builder = new MapperConfigurationBuilder();

        // Act
        var expression = builder.CreateMap<Source, Destination>()
            .ForMember(d => d.TargetName, opt => opt.MapFrom(s => s.SourceName))
            .ForMember(d => d.TargetAge, opt => opt.MapFrom(s => s.SourceAge))
            .ForMember(d => d.IgnoredProperty, opt => opt.Ignore());

        // Assert
        var mappingExpr = builder.GetMappingExpression<Source, Destination>();
        mappingExpr.Should().NotBeNull();
        mappingExpr!.MemberConfigurations.Should().HaveCount(3);

        var targetNameConfig = mappingExpr.MemberConfigurations.First(m => m.MemberName == "TargetName");
        targetNameConfig.SourceMemberName.Should().Be("SourceName");

        var targetAgeConfig = mappingExpr.MemberConfigurations.First(m => m.MemberName == "TargetAge");
        targetAgeConfig.SourceMemberName.Should().Be("SourceAge");

        var ignoredConfig = mappingExpr.MemberConfigurations.First(m => m.MemberName == "IgnoredProperty");
        ignoredConfig.IsIgnored.Should().BeTrue();
    }

    [Fact]
    public void CreateMap_MultipleMappings_ShouldMaintainAllConfigurations()
    {
        // Arrange
        var builder = new MapperConfigurationBuilder();

        // Act
        builder.CreateMap<Source, Destination>()
            .ForMember(d => d.TargetName, opt => opt.MapFrom(s => s.SourceName));

        builder.CreateMap<Person, PersonDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.Name));

        // Assert
        builder.MappingExpressions.Should().HaveCount(2);

        var sourceDestMapping = builder.GetMappingExpression<Source, Destination>();
        sourceDestMapping.Should().NotBeNull();
        sourceDestMapping!.MemberConfigurations.Should().HaveCount(1);

        var personMapping = builder.GetMappingExpression<Person, PersonDto>();
        personMapping.Should().NotBeNull();
        personMapping!.MemberConfigurations.Should().HaveCount(1);
    }

    [Fact]
    public void MappingExpression_ShouldExposeSourceAndDestinationTypes()
    {
        // Arrange
        var builder = new MapperConfigurationBuilder();

        // Act
        builder.CreateMap<Source, Destination>();

        // Assert
        var mappingExpr = builder.GetMappingExpression<Source, Destination>();
        mappingExpr.Should().NotBeNull();
        mappingExpr!.SourceType.Should().Be(typeof(Source));
        mappingExpr.DestinationType.Should().Be(typeof(Destination));
    }

    [Fact]
    public void ForMember_WithNullDestinationMember_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new MapperConfigurationBuilder();
        var expression = builder.CreateMap<Source, Destination>();

        // Act
        Action act = () => expression.ForMember<string>(null!, opt => opt.Ignore());

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ForMember_WithNullMemberOptions_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new MapperConfigurationBuilder();
        var expression = builder.CreateMap<Source, Destination>();

        // Act
        Action act = () => expression.ForMember(d => d.TargetName, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MapFrom_WithNullSourceMember_ShouldThrowArgumentNullException()
    {
        // Arrange
        var builder = new MapperConfigurationBuilder();
        var expression = builder.CreateMap<Source, Destination>();

        // Act & Assert
        var exception = Assert.Throws<ArgumentNullException>(() =>
            expression.ForMember(d => d.TargetName, opt => opt.MapFrom(null!))
        );

        exception.ParamName.Should().Be("sourceMember");
    }

    [Fact]
    public void GetMappingExpression_NonExistentMapping_ShouldReturnNull()
    {
        // Arrange
        var builder = new MapperConfigurationBuilder();
        builder.CreateMap<Source, Destination>();

        // Act
        var result = builder.GetMappingExpression<Person, PersonDto>();

        // Assert
        result.Should().BeNull();
    }

    // Test Models
    public class Source
    {
        public string SourceName { get; set; } = string.Empty;
        public int SourceAge { get; set; }
    }

    public class Destination
    {
        public string TargetName { get; set; } = string.Empty;
        public int TargetAge { get; set; }
        public string IgnoredProperty { get; set; } = string.Empty;
    }

    public class Person
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    public class PersonDto
    {
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; }
    }
}
