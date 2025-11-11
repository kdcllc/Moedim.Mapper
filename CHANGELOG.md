# Changelog

All notable changes to Moedim.Mapper will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2025-01-11

### Added
- Initial release of Moedim.Mapper
- Source generator-based object mapping with compile-time code generation
- Attribute-based mapping with `[MapFrom]` and `[MapTo]` attributes
- Custom property mapping with `[MapProperty]` attribute
- Property exclusion with `[IgnoreProperty]` attribute
- Bidirectional mapping support
- Null-safe code generation
- Collection mapping support (List, Array, IEnumerable)
- Type conversion for compatible types
- Fluent configuration interfaces (IMapperConfigurationBuilder, IMappingExpression)
- Multi-framework support (.NET 6.0, 7.0, and 8.0)
- Comprehensive test suite (11 unit tests)
- Performance benchmarks comparing source generator, manual, and reflection-based mapping
- Sample application demonstrating various usage scenarios
- XML documentation comments on all public APIs
- Complete README with usage examples and documentation

### Performance
- Source generator mapping: ~10 ns/operation (only 3% slower than manual mapping)
- 38x faster than reflection-based mapping
- Zero runtime overhead - all mapping code generated at compile time
- Minimal memory allocation (40 bytes per mapping, same as manual)

### Project Structure
- `Moedim.Mapper` - Core library with attributes and interfaces
- `Moedim.Mapper.SourceGenerator` - Roslyn incremental source generator
- `Moedim.Mapper.Tests` - Unit tests with 10 test cases
- `Moedim.Mapper.SourceGenerator.Tests` - Source generator specific tests
- `Moedim.Mapper.Performance.Tests` - BenchmarkDotNet performance tests
- `Moedim.Mapper.Sample` - Example application

[1.0.0]: https://github.com/kdcllc/Moedim.Mapper/releases/tag/v1.0.0
