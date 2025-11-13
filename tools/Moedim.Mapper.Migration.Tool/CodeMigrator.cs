using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Moedim.Mapper.Migration.Tool;

public static class CodeMigrator
{
    public static async Task MigrateAsync(string path, string strategy, bool dryRun, bool backup)
    {
        Console.WriteLine($"🔄 Migrating AutoMapper to Moedim.Mapper");
        Console.WriteLine($"   Strategy: {strategy}");
        Console.WriteLine($"   Dry run: {dryRun}");
        Console.WriteLine($"   Backup: {backup}");
        Console.WriteLine();

        var files = Directory.GetFiles(Path.GetDirectoryName(path) ?? ".", "*.cs", SearchOption.AllDirectories);
        var migratedCount = 0;

        foreach (var file in files)
        {
            if (file.Contains("/obj/") || file.Contains("\\obj\\") ||
                file.Contains("/bin/") || file.Contains("\\bin\\"))
                continue;

            var code = await File.ReadAllTextAsync(file);

            if (!code.Contains("AutoMapper") && !code.Contains("Profile"))
                continue;

            Console.WriteLine($"Processing: {Path.GetFileName(file)}");

            var newCode = strategy.ToLower() == "fluent"
                ? MigrateToFluentApi(code, file)
                : MigrateToAttributes(code, file);

            if (newCode != code)
            {
                migratedCount++;

                if (dryRun)
                {
                    Console.WriteLine($"  Would migrate: {Path.GetFileName(file)}");
                }
                else
                {
                    if (backup)
                    {
                        await File.WriteAllTextAsync(file + ".bak", code);
                    }

                    await File.WriteAllTextAsync(file, newCode);
                    Console.WriteLine($"  ✓ Migrated: {Path.GetFileName(file)}");
                }
            }
        }

        Console.WriteLine();
        Console.WriteLine($"{'━',60}");
        Console.WriteLine($"Migration complete: {migratedCount} files {(dryRun ? "would be" : "")} modified");

        if (!dryRun)
        {
            Console.WriteLine();
            Console.WriteLine("⚠️  Next steps:");
            Console.WriteLine("   1. Remove AutoMapper package: dotnet remove package AutoMapper");
            Console.WriteLine("   2. Add Moedim.Mapper package: dotnet add package Moedim.Mapper");
            Console.WriteLine("   3. Build and test your project");
            Console.WriteLine("   4. Review migration warnings in comments");
        }
    }

    private static string MigrateToAttributes(string code, string filePath)
    {
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = tree.GetRoot();

        // Replace using statements
        code = code.Replace("using AutoMapper;", "using Moedim.Mapper;");

        // Add comment about manual migration needed
        if (code.Contains("Profile") && code.Contains("CreateMap"))
        {
            code = "// TODO: Migrate AutoMapper Profile to Moedim.Mapper attributes or fluent API\n" +
                   "// Use [MapFrom(typeof(SourceType))] or [MapTo(typeof(DestType))] on classes\n" +
                   "// Use [MapProperty(\"SourcePropertyName\")] for custom property mappings\n" +
                   "// Use [IgnoreProperty] to exclude properties\n" +
                   code;
        }

        return code;
    }

    private static string MigrateToFluentApi(string code, string filePath)
    {
        // Replace using statements
        code = code.Replace("using AutoMapper;", "using Moedim.Mapper;");

        // Replace Profile base class
        code = code.Replace(": Profile", ": object // TODO: Implement IMapperConfiguration");

        // Replace CreateMap method calls
        code = code.Replace("CreateMap<", "builder.CreateMap<");

        // Add TODO comments for manual review
        if (code.Contains("Profile"))
        {
            code = "// TODO: Convert AutoMapper Profile to Moedim.Mapper configuration\n" +
                   "// 1. Remove Profile base class\n" +
                   "// 2. Add [MapperConfiguration] attribute\n" +
                   "// 3. Create Configure(IMapperConfigurationBuilder builder) method\n" +
                   "// 4. Use builder.CreateMap<TSource, TDest>().ForMember(...)\n" +
                   code;
        }

        return code;
    }
}
