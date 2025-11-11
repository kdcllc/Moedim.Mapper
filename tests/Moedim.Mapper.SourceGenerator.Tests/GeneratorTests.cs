using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using FluentAssertions;

namespace Moedim.Mapper.SourceGenerator.Tests;

/// <summary>
/// Tests for the MapperGenerator source generator.
/// </summary>
public class GeneratorTests
{
    [Fact]
    public void Generator_WithMapFromAttribute_GeneratesExtensionMethod()
    {
        // Arrange
        var source = @"
using Moedim.Mapper;

namespace TestNamespace
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    [MapFrom(typeof(Person))]
    public class PersonDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}";

        // Act
        var (diagnostics, output) = GetGeneratedOutput(source);

        // Assert
        diagnostics.Should().BeEmpty();
        output.Should().Contain("ToPersonDto");
        output.Should().Contain("public static class PersonToPersonDtoMappingExtensions");
    }

    private static (ImmutableArray<Diagnostic> Diagnostics, string Output) GetGeneratedOutput(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(assembly => !assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .Cast<MetadataReference>();

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new MapperGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var diagnostics);

        var runResult = driver.GetRunResult();
        var generatedSource = runResult.Results[0].GeneratedSources.Length > 0
            ? runResult.Results[0].GeneratedSources[0].SourceText.ToString()
            : string.Empty;

        return (diagnostics, generatedSource);
    }
}
