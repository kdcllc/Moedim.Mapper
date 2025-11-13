using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Moedim.Mapper.Migration.Tool;

public static class MigrationAnalyzer
{
    public static async Task AnalyzeAsync(string path, bool verbose, string? outputFile)
    {
        Console.WriteLine($"🔍 Analyzing AutoMapper usage in: {path}");
        Console.WriteLine();

        var files = Directory.GetFiles(Path.GetDirectoryName(path) ?? ".", "*.cs", SearchOption.AllDirectories);

        var analysis = new AnalysisResult();

        foreach (var file in files)
        {
            if (file.Contains("/obj/") || file.Contains("\\obj\\") ||
                file.Contains("/bin/") || file.Contains("\\bin\\"))
                continue;

            var code = await File.ReadAllTextAsync(file);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = await tree.GetRootAsync();

            AnalyzeFile(file, root, analysis, verbose);
        }

        PrintAnalysis(analysis);

        if (outputFile != null)
        {
            await File.WriteAllTextAsync(outputFile, GenerateAnalysisReport(analysis));
            Console.WriteLine($"\n📄 Report saved to: {outputFile}");
        }
    }

    private static void AnalyzeFile(string filePath, SyntaxNode root, AnalysisResult analysis, bool verbose)
    {
        // Find AutoMapper usings
        var usingDirectives = root.DescendantNodes().OfType<UsingDirectiveSyntax>();
        foreach (var usingDirective in usingDirectives)
        {
            if (usingDirective.Name?.ToString() == "AutoMapper")
            {
                analysis.FilesWithAutoMapper.Add(filePath);
                if (verbose)
                    Console.WriteLine($"  Found AutoMapper using in: {Path.GetFileName(filePath)}");
            }
        }

        // Find Profile classes
        var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
        foreach (var classDecl in classes)
        {
            var baseList = classDecl.BaseList?.Types.Select(t => t.ToString()) ?? Enumerable.Empty<string>();
            if (baseList.Any(b => b == "Profile" || b.Contains("AutoMapper.Profile")))
            {
                analysis.ProfileClasses.Add(new ProfileInfo
                {
                    FilePath = filePath,
                    ClassName = classDecl.Identifier.Text
                });
                if (verbose)
                    Console.WriteLine($"  Found Profile class: {classDecl.Identifier.Text}");
            }
        }

        // Find CreateMap calls
        var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();
        foreach (var invocation in invocations)
        {
            var methodName = invocation.Expression.ToString();
            if (methodName.Contains("CreateMap"))
            {
                analysis.CreateMapCalls++;
                if (verbose)
                    Console.WriteLine($"  Found CreateMap call in: {Path.GetFileName(filePath)}");
            }
            else if (methodName.Contains("ForMember"))
            {
                analysis.ForMemberCalls++;
            }
            else if (methodName.Contains(".Ignore("))
            {
                analysis.IgnoreCalls++;
            }
            else if (methodName.Contains("ConvertUsing") || methodName.Contains("ResolveUsing"))
            {
                analysis.CustomConverterCalls++;
            }
        }

        // Find IMapper injections
        var properties = root.DescendantNodes().OfType<PropertyDeclarationSyntax>();
        var fields = root.DescendantNodes().OfType<FieldDeclarationSyntax>();
        var parameters = root.DescendantNodes().OfType<ParameterSyntax>();

        foreach (var prop in properties)
        {
            if (prop.Type.ToString().Contains("IMapper"))
            {
                analysis.MapperInjections++;
            }
        }

        foreach (var field in fields)
        {
            if (field.Declaration.Type.ToString().Contains("IMapper"))
            {
                analysis.MapperInjections++;
            }
        }

        foreach (var param in parameters)
        {
            if (param.Type?.ToString().Contains("IMapper") == true)
            {
                analysis.MapperInjections++;
            }
        }
    }

    private static void PrintAnalysis(AnalysisResult analysis)
    {
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine("                    ANALYSIS SUMMARY                       ");
        Console.WriteLine("═══════════════════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine($"📁 Files with AutoMapper: {analysis.FilesWithAutoMapper.Count}");
        Console.WriteLine($"📋 Profile classes: {analysis.ProfileClasses.Count}");
        Console.WriteLine($"🔧 CreateMap calls: {analysis.CreateMapCalls}");
        Console.WriteLine($"⚙️  ForMember calls: {analysis.ForMemberCalls}");
        Console.WriteLine($"🚫 Ignore calls: {analysis.IgnoreCalls}");
        Console.WriteLine($"🔄 Custom converters: {analysis.CustomConverterCalls}");
        Console.WriteLine($"💉 IMapper injections: {analysis.MapperInjections}");
        Console.WriteLine();

        if (analysis.ProfileClasses.Any())
        {
            Console.WriteLine("Profile Classes Found:");
            foreach (var profile in analysis.ProfileClasses)
            {
                Console.WriteLine($"  • {profile.ClassName} ({Path.GetFileName(profile.FilePath)})");
            }
            Console.WriteLine();
        }

        Console.WriteLine("Migration Complexity: " + GetComplexity(analysis));
        Console.WriteLine("Recommended Strategy: " + GetRecommendedStrategy(analysis));
        Console.WriteLine();
    }

    private static string GetComplexity(AnalysisResult analysis)
    {
        var totalCalls = analysis.CreateMapCalls + analysis.ForMemberCalls + analysis.CustomConverterCalls;

        if (totalCalls < 10) return "🟢 LOW - Quick migration expected";
        if (totalCalls < 50) return "🟡 MEDIUM - Moderate effort required";
        return "🔴 HIGH - Significant refactoring needed";
    }

    private static string GetRecommendedStrategy(AnalysisResult analysis)
    {
        if (analysis.ProfileClasses.Count > 0)
            return "Fluent API (convert Profile classes)";
        return "Attributes (add to DTOs)";
    }

    private static string GenerateAnalysisReport(AnalysisResult analysis)
    {
        return $@"# AutoMapper Migration Analysis Report

## Summary
- Files with AutoMapper: {analysis.FilesWithAutoMapper.Count}
- Profile classes: {analysis.ProfileClasses.Count}
- CreateMap calls: {analysis.CreateMapCalls}
- ForMember calls: {analysis.ForMemberCalls}
- Ignore calls: {analysis.IgnoreCalls}
- Custom converters: {analysis.CustomConverterCalls}
- IMapper injections: {analysis.MapperInjections}

## Migration Complexity
{GetComplexity(analysis)}

## Recommended Strategy
{GetRecommendedStrategy(analysis)}

## Profile Classes
{string.Join("\n", analysis.ProfileClasses.Select(p => $"- {p.ClassName} ({p.FilePath})"))}

## Files to Review
{string.Join("\n", analysis.FilesWithAutoMapper.Select(f => $"- {f}"))}
";
    }
}

internal class AnalysisResult
{
    public HashSet<string> FilesWithAutoMapper { get; } = new();
    public List<ProfileInfo> ProfileClasses { get; } = new();
    public int CreateMapCalls { get; set; }
    public int ForMemberCalls { get; set; }
    public int IgnoreCalls { get; set; }
    public int CustomConverterCalls { get; set; }
    public int MapperInjections { get; set; }
}

internal class ProfileInfo
{
    public string FilePath { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
}
