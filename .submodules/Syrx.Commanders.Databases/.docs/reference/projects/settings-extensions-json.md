# Syrx.Commanders.Databases.Settings.Extensions.Json

## Summary
JSON configuration extension package that adds trusted JSON settings files to the Syrx configuration pipeline.

## Key Types and Interfaces
- `UseFileExtensions.UseFile(this SyrxBuilder, string fileName, IConfigurationBuilder builder)`.
- `ServiceCollectionExtensions` (legacy compatibility type with no active registration methods).

## Usage Notes
- `UseFile(...)` enforces:
  - non-null configuration builder;
  - non-empty file name;
  - leaf filename only (no `/` or `\\`);
  - `.json` extension.
- On success, it calls `IConfigurationBuilder.AddJsonFile(fileName)` and returns the same `SyrxBuilder`.

## Thread Safety
- Static extension methods are stateless and thread-safe.
- Intended for application startup configuration, not runtime mutation.

## Security Considerations
- Filename validation prevents path traversal via directory separators.
- Validation does not verify file contents; protect file location and change control.
- Keep secrets outside source-controlled JSON whenever possible.

## Performance Notes
- Minimal startup overhead; one filename validation and one configuration registration call.
- No recurring runtime cost after configuration is built.

## See Also
- [Syrx.Commanders.Databases.Settings.Extensions.Xml](./settings-extensions-xml.md)
- [Syrx.Commanders.Databases.Settings](./settings.md)
- [Configuration Overview](../configuration/index.md)
