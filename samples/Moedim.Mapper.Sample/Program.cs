using Moedim.Mapper.Sample.Examples;

namespace Moedim.Mapper.Sample;

/// <summary>
/// Sample program demonstrating Moedim.Mapper usage including complex object mapping.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         Moedim.Mapper Sample Application                   ║");
        Console.WriteLine("║    Demonstrating Complex Object Mapping Capabilities       ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        try
        {
            // Run basic mapping examples
            BasicMappingExamples.Run();

            Console.WriteLine(new string('─', 60));
            Console.WriteLine();

            // Run complex mapping examples
            ComplexMappingExamples.Run();

            Console.WriteLine(new string('═', 60));
            Console.WriteLine("✓ All examples completed successfully!");
            Console.WriteLine("═".PadRight(60, '═'));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n✗ Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }
}
