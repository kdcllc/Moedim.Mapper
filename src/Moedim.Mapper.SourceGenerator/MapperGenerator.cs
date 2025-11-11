using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Moedim.Mapper.SourceGenerator
{

/// <summary>
/// Source generator for creating object mapping extension methods.
/// </summary>
[Generator]
public class MapperGenerator : IIncrementalGenerator
{
    private const string MapFromAttributeName = "Moedim.Mapper.MapFromAttribute";
    private const string MapToAttributeName = "Moedim.Mapper.MapToAttribute";
    private const string MapPropertyAttributeName = "Moedim.Mapper.MapPropertyAttribute";
    private const string IgnorePropertyAttributeName = "Moedim.Mapper.IgnorePropertyAttribute";

    /// <summary>
    /// Initializes the generator.
    /// </summary>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Register attribute-based mappings
        var attributeBasedMappings = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsCandidateClass(node),
                transform: static (ctx, _) => GetMappingInfo(ctx))
            .Where(static m => m is not null);

        // Register convention-based mappings (partial classes)
        var conventionBasedMappings = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsPartialClass(node),
                transform: static (ctx, _) => GetPartialClassInfo(ctx))
            .Where(static m => m is not null);

        // Combine and generate
        var allMappings = attributeBasedMappings.Combine(conventionBasedMappings.Collect());

        context.RegisterSourceOutput(allMappings, static (spc, source) => Execute(spc, source.Left, source.Right));
    }

    private static bool IsCandidateClass(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDecl && classDecl.AttributeLists.Count > 0;
    }

    private static bool IsPartialClass(SyntaxNode node)
    {
        if (node is not ClassDeclarationSyntax classDecl)
            return false;

        return classDecl.Modifiers.Any(m => m.ValueText == "partial");
    }

    private static MappingInfo? GetMappingInfo(GeneratorSyntaxContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;

        if (symbol is null)
            return null;

        var mapFromAttrs = symbol.GetAttributes()
            .Where(a => a.AttributeClass?.ToDisplayString() == MapFromAttributeName)
            .ToList();

        var mapToAttrs = symbol.GetAttributes()
            .Where(a => a.AttributeClass?.ToDisplayString() == MapToAttributeName)
            .ToList();

        if (mapFromAttrs.Count == 0 && mapToAttrs.Count == 0)
            return null;

        return new MappingInfo
        {
            TargetType = symbol,
            MapFromAttributes = mapFromAttrs,
            MapToAttributes = mapToAttrs,
            IsAttributeBased = true
        };
    }

    private static MappingInfo? GetPartialClassInfo(GeneratorSyntaxContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;
        var symbol = context.SemanticModel.GetDeclaredSymbol(classDecl) as INamedTypeSymbol;

        if (symbol is null)
            return null;

        // For convention-based, we'll look for similar types in the same namespace
        return new MappingInfo
        {
            TargetType = symbol,
            MapFromAttributes = ImmutableArray<AttributeData>.Empty,
            MapToAttributes = ImmutableArray<AttributeData>.Empty,
            IsAttributeBased = false
        };
    }

    private static void Execute(
        SourceProductionContext context,
        MappingInfo? attributeBased,
        ImmutableArray<MappingInfo?> conventionBased)
    {
        // Process attribute-based mappings
        if (attributeBased is not null)
        {
            GenerateAttributeBasedMappings(context, attributeBased);
        }

        // Process convention-based mappings
        foreach (var mapping in conventionBased)
        {
            if (mapping is not null)
            {
                GenerateConventionBasedMappings(context, mapping);
            }
        }
    }

    private static void GenerateAttributeBasedMappings(SourceProductionContext context, MappingInfo mappingInfo)
    {
        if (mappingInfo.TargetType is null || mappingInfo.MapFromAttributes is null || mappingInfo.MapToAttributes is null)
            return;

        // Generate mappings based on MapFrom and MapTo attributes
        foreach (var mapFromAttr in mappingInfo.MapFromAttributes)
        {
            if (mapFromAttr.ConstructorArguments.Length > 0)
            {
                var sourceType = mapFromAttr.ConstructorArguments[0].Value as INamedTypeSymbol;
                if (sourceType is not null)
                {
                    var code = GenerateMappingExtension(sourceType, mappingInfo.TargetType, mappingInfo.TargetType);
                    context.AddSource($"{sourceType.Name}To{mappingInfo.TargetType.Name}Extensions.g.cs", code);
                }
            }
        }

        foreach (var mapToAttr in mappingInfo.MapToAttributes)
        {
            if (mapToAttr.ConstructorArguments.Length > 0)
            {
                var destType = mapToAttr.ConstructorArguments[0].Value as INamedTypeSymbol;
                if (destType is not null)
                {
                    var code = GenerateMappingExtension(mappingInfo.TargetType, destType, mappingInfo.TargetType);
                    context.AddSource($"{mappingInfo.TargetType.Name}To{destType.Name}Extensions.g.cs", code);
                }
            }
        }
    }

    private static void GenerateConventionBasedMappings(SourceProductionContext context, MappingInfo mappingInfo)
    {
        // Convention-based: look for types with similar names
        // For now, skip this to focus on attribute-based approach
        // This would require analyzing all types in the compilation
    }

    private static string GenerateMappingExtension(
        INamedTypeSymbol sourceType,
        INamedTypeSymbol destType,
        INamedTypeSymbol contextType)
    {
        var sourceTypeName = sourceType.ToDisplayString();
        var destTypeName = destType.ToDisplayString();
        var sourceSimpleName = sourceType.Name;
        var destSimpleName = destType.Name;

        var sourceNamespace = sourceType.ContainingNamespace.ToDisplayString();
        var destNamespace = destType.ContainingNamespace.ToDisplayString();

        var sb = new StringBuilder();

        // Add file header
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();

        // Add usings
        var namespaces = new HashSet<string> { sourceNamespace, destNamespace };
        foreach (var ns in namespaces.Where(n => !string.IsNullOrEmpty(n) && n != "<global namespace>").OrderBy(n => n))
        {
            sb.AppendLine($"using {ns};");
        }
        sb.AppendLine();

        // Generate extension class
        var extensionClassName = $"{sourceSimpleName}To{destSimpleName}MappingExtensions";
        sb.AppendLine($"namespace {sourceNamespace};");
        sb.AppendLine();
        sb.AppendLine($"/// <summary>");
        sb.AppendLine($"/// Extension methods for mapping {sourceSimpleName} to {destSimpleName}.");
        sb.AppendLine($"/// </summary>");
        sb.AppendLine($"public static class {extensionClassName}");
        sb.AppendLine("{");

        // Generate To method
        sb.AppendLine($"    /// <summary>");
        sb.AppendLine($"    /// Maps {sourceSimpleName} to {destSimpleName}.");
        sb.AppendLine($"    /// </summary>");
        sb.AppendLine($"    public static {destTypeName}? To{destSimpleName}(this {sourceTypeName}? source)");
        sb.AppendLine("    {");
        sb.AppendLine("        if (source is null)");
        sb.AppendLine("            return null;");
        sb.AppendLine();
        sb.AppendLine($"        return new {destTypeName}");
        sb.AppendLine("        {");

        // Map properties
        var propertiesToMap = GetMappableProperties(sourceType, destType, contextType);
        for (int i = 0; i < propertiesToMap.Count; i++)
        {
            var (sourceProp, destProp) = propertiesToMap[i];
            var comma = i < propertiesToMap.Count - 1 ? "," : "";
            
            var mapping = GeneratePropertyMapping(sourceProp, destProp, "source");
            sb.AppendLine($"            {destProp.Name} = {mapping}{comma}");
        }

        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static List<(IPropertySymbol Source, IPropertySymbol Dest)> GetMappableProperties(
        INamedTypeSymbol sourceType,
        INamedTypeSymbol destType,
        INamedTypeSymbol contextType)
    {
        var result = new List<(IPropertySymbol, IPropertySymbol)>();

        var sourceProps = sourceType.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public && p.GetMethod is not null)
            .ToList();

        var destProps = destType.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public && p.SetMethod is not null)
            .ToList();

        foreach (var destProp in destProps)
        {
            // Check for IgnoreProperty attribute
            if (destProp.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == IgnorePropertyAttributeName))
                continue;

            // Check for MapProperty attribute
            var mapPropAttr = destProp.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == MapPropertyAttributeName);

            IPropertySymbol? sourceProp = null;

            if (mapPropAttr is not null && mapPropAttr.ConstructorArguments.Length > 0)
            {
                var sourcePropName = mapPropAttr.ConstructorArguments[0].Value as string;
                sourceProp = sourceProps.FirstOrDefault(p => p.Name == sourcePropName);
            }
            else
            {
                // Convention-based matching
                sourceProp = sourceProps.FirstOrDefault(p => p.Name == destProp.Name);
            }

            if (sourceProp is not null && AreTypesCompatible(sourceProp.Type, destProp.Type))
            {
                result.Add((sourceProp, destProp));
            }
        }

        return result;
    }

    private static bool AreTypesCompatible(ITypeSymbol sourceType, ITypeSymbol destType)
    {
        // Direct match
        if (SymbolEqualityComparer.Default.Equals(sourceType, destType))
            return true;

        // Nullable compatibility
        var sourceNonNullable = IsNullableValueType(sourceType) 
            ? ((INamedTypeSymbol)sourceType).TypeArguments[0] 
            : sourceType;
        var destNonNullable = IsNullableValueType(destType) 
            ? ((INamedTypeSymbol)destType).TypeArguments[0] 
            : destType;

        if (SymbolEqualityComparer.Default.Equals(sourceNonNullable, destNonNullable))
            return true;

        // Collection compatibility
        if (IsEnumerableType(sourceType) && IsEnumerableType(destType))
            return true;

        // Numeric conversions (int to long, etc.)
        if (IsNumericType(sourceNonNullable) && IsNumericType(destNonNullable))
            return true;

        return false;
    }

    private static bool IsNullableValueType(ITypeSymbol type)
    {
        return type is INamedTypeSymbol named &&
               named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
    }

    private static bool IsEnumerableType(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol)
            return true;

        if (type is INamedTypeSymbol named)
        {
            var typeName = named.OriginalDefinition.ToDisplayString();
            return typeName.StartsWith("System.Collections.Generic.IEnumerable<") ||
                   typeName.StartsWith("System.Collections.Generic.List<") ||
                   typeName.StartsWith("System.Collections.Generic.IList<") ||
                   typeName.StartsWith("System.Collections.Generic.ICollection<");
        }

        return false;
    }

    private static bool IsNumericType(ITypeSymbol type)
    {
        return type.SpecialType switch
        {
            SpecialType.System_Byte => true,
            SpecialType.System_SByte => true,
            SpecialType.System_Int16 => true,
            SpecialType.System_UInt16 => true,
            SpecialType.System_Int32 => true,
            SpecialType.System_UInt32 => true,
            SpecialType.System_Int64 => true,
            SpecialType.System_UInt64 => true,
            SpecialType.System_Single => true,
            SpecialType.System_Double => true,
            SpecialType.System_Decimal => true,
            _ => false
        };
    }

    private static string GeneratePropertyMapping(IPropertySymbol sourceProp, IPropertySymbol destProp, string sourceVar)
    {
        var sourceAccess = $"{sourceVar}.{sourceProp.Name}";

        // Handle collections
        if (IsEnumerableType(sourceProp.Type) && IsEnumerableType(destProp.Type))
        {
            if (sourceProp.Type is IArrayTypeSymbol sourceArray && destProp.Type is IArrayTypeSymbol destArray)
            {
                // Array to array
                return $"{sourceAccess}";
            }
            else if (destProp.Type is INamedTypeSymbol destNamed)
            {
                var destTypeName = destNamed.OriginalDefinition.ToDisplayString();
                if (destTypeName.StartsWith("System.Collections.Generic.List<"))
                {
                    // To List
                    return $"{sourceAccess}?.ToList()";
                }
                else if (destTypeName.StartsWith("System.Collections.Generic.IEnumerable<"))
                {
                    // To IEnumerable
                    return $"{sourceAccess}";
                }
            }
            
            return $"{sourceAccess}?.ToList()";
        }

        // Handle nullable types
        if (IsNullableValueType(destProp.Type) && !IsNullableValueType(sourceProp.Type))
        {
            return sourceAccess;
        }

        // Direct assignment
        return sourceAccess;
    }

    private class MappingInfo
    {
        public INamedTypeSymbol? TargetType { get; set; }
        public IEnumerable<AttributeData>? MapFromAttributes { get; set; }
        public IEnumerable<AttributeData>? MapToAttributes { get; set; }
        public bool IsAttributeBased { get; set; }
    }
}
}
