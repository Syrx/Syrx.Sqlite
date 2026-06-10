# Syrx.Commanders.Databases.Settings.Extensions.Xml

## Summary
XML configuration extension package that adds trusted XML settings files to the Syrx configuration pipeline.

## Key Types and Interfaces
- `UseFileExtensions.UseFile(this SyrxBuilder, string fileName, IConfigurationBuilder builder)`.
- `ServiceCollectionExtensions` (legacy compatibility type with no active registration methods).

## Usage Notes
- `UseFile(...)` enforces:
  - non-null configuration builder;
  - non-empty file name;
  - leaf filename only (no `/` or `\\`);
  - `.xml` extension.
- On success, it calls `IConfigurationBuilder.AddXmlFile(fileName)` and returns the same `SyrxBuilder`.

## Thread Safety
- Static extension methods are stateless and thread-safe.
- Intended for startup pipeline composition.

## Security Considerations
- Filename validation blocks directory traversal through path separators.
- File content trust and secret handling remain caller responsibilities.

## Performance Notes
- Startup-only registration path with negligible overhead.
- No per-request processing in this package.

## See Also
- [Syrx.Commanders.Databases.Settings.Extensions.Json](./settings-extensions-json.md)
- [Syrx.Commanders.Databases.Settings](./settings.md)
- [Configuration Overview](../configuration/index.md)
