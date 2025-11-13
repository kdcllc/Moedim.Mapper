# Moedim.Mapper Migration Tool

A command-line tool to help migrate from AutoMapper to Moedim.Mapper.

## Installation

### As a Global Tool

```bash
dotnet tool install -g Moedim.Mapper.Migration.Tool
```

### From Source

```bash
cd tools/Moedim.Mapper.Migration.Tool
dotnet pack
dotnet tool install -g --add-source ./bin/Release Moedim.Mapper.Migration.Tool
```

## Usage

### Analyze Project

Analyze your project or solution for AutoMapper usage:

```bash
moedim-mapper-migrate analyze path/to/YourProject.csproj
```

With verbose output:

```bash
moedim-mapper-migrate analyze path/to/YourSolution.sln --verbose
```

Save analysis to file:

```bash
moedim-mapper-migrate analyze path/to/YourProject.csproj --output analysis.txt
```

### Migrate Code

Migrate your AutoMapper code to Moedim.Mapper using attributes (recommended):

```bash
moedim-mapper-migrate migrate path/to/YourProject.csproj
```

Use fluent API instead:

```bash
moedim-mapper-migrate migrate path/to/YourProject.csproj --strategy fluent
```

Dry run (preview changes without applying):

```bash
moedim-mapper-migrate migrate path/to/YourProject.csproj --dry-run
```

Skip backup:

```bash
moedim-mapper-migrate migrate path/to/YourProject.csproj --backup false
```

### Generate Report

Generate a compatibility report:

```bash
moedim-mapper-migrate report path/to/YourProject.csproj
```

Generate HTML report:

```bash
moedim-mapper-migrate report path/to/YourProject.csproj --format html --output report.html
```

Generate JSON report:

```bash
moedim-mapper-migrate report path/to/YourProject.csproj --format json --output report.json
```

## What It Does

### Analyze Command

The analyze command scans your codebase for:

- AutoMapper Profile classes
- `CreateMap<TSource, TDest>()` calls
- `ForMember()` configurations
- `Ignore()` configurations
- `IMapper` interface usage
- AutoMapper NuGet package references

### Migrate Command

The migrate command performs the following transformations:

#### Attributes Strategy (Default)

Converts AutoMapper Profile classes to Moedim.Mapper attributes:

**Before:**
```csharp
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => s.FirstName + " " + s.LastName))
            .ForMember(d => d.Age, opt => opt.Ignore());
    }
}
```

**After:**
```csharp
[MapTo(typeof(UserDto))]
public partial class User
{
    [MapProperty("FullName", "FirstName + \" \" + LastName")]
    public string FirstName { get; set; }

    public string LastName { get; set; }

    [IgnoreProperty]
    public int Age { get; set; }
}
```

#### Fluent API Strategy

Converts to Moedim.Mapper fluent configuration:

```csharp
public class UserMapperConfiguration : IMapperConfiguration
{
    public static void Configure()
    {
        MapperConfigurationBuilder.Create()
            .For<User, UserDto>()
                .MapProperty(d => d.FullName, s => s.FirstName + " " + s.LastName)
                .Ignore(d => d.Age)
            .Build();
    }
}
```

### Report Command

Generates a comprehensive migration report including:

- Total AutoMapper usages found
- Breakdown by pattern type (Profiles, CreateMap, ForMember, etc.)
- File-by-file analysis
- Compatibility assessment
- Recommended migration strategy
- Estimated migration effort

## Options

### Common Options

- `-h, --help` - Show help information
- `--version` - Show version information

### Analyze Options

- `-v, --verbose` - Show detailed analysis with line numbers and code snippets
- `-o, --output <file>` - Save analysis results to file

### Migrate Options

- `-s, --strategy <strategy>` - Migration strategy: `attributes` (default) or `fluent`
- `-d, --dry-run` - Preview changes without applying them
- `-b, --backup <true|false>` - Create backup before migration (default: true)

### Report Options

- `-f, --format <format>` - Report format: `markdown` (default), `json`, or `html`
- `-o, --output <file>` - Save report to file

## Manual Migration

For manual migration guidance, see [MIGRATION_GUIDE.md](../../MIGRATION_GUIDE.md).

## Development

Build from source:

```bash
dotnet build
```

Run locally:

```bash
dotnet run -- analyze path/to/project.csproj
```

Pack as tool:

```bash
dotnet pack -c Release
```

## License

MIT
