using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Moedim.Mapper.Performance.Tests;

/// <summary>
/// Benchmark models.
/// </summary>
public class BenchmarkPerson
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
}

[MapFrom(typeof(BenchmarkPerson))]
public class BenchmarkPersonDto
{
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
}

/// <summary>
/// Benchmark comparing different mapping approaches.
/// </summary>
[MemoryDiagnoser]
public class MappingBenchmarks
{
    private BenchmarkPerson _person = null!;

    [GlobalSetup]
    public void Setup()
    {
        _person = new BenchmarkPerson
        {
            Name = "John Doe",
            Age = 30,
            Email = "john@example.com"
        };
    }

    [Benchmark(Baseline = true)]
    public BenchmarkPersonDto ManualMapping()
    {
        return new BenchmarkPersonDto
        {
            Name = _person.Name,
            Age = _person.Age,
            Email = _person.Email
        };
    }

    [Benchmark]
    public BenchmarkPersonDto? SourceGeneratorMapping()
    {
        return _person.ToBenchmarkPersonDto();
    }

    [Benchmark]
    public BenchmarkPersonDto ReflectionMapping()
    {
        var dto = new BenchmarkPersonDto();
        var sourceProps = typeof(BenchmarkPerson).GetProperties();
        var destProps = typeof(BenchmarkPersonDto).GetProperties();

        foreach (var sourceProp in sourceProps)
        {
            var destProp = destProps.FirstOrDefault(p => p.Name == sourceProp.Name);
            if (destProp != null && destProp.CanWrite)
            {
                destProp.SetValue(dto, sourceProp.GetValue(_person));
            }
        }

        return dto;
    }
}

/// <summary>
/// Program entry point.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<MappingBenchmarks>();
    }
}
