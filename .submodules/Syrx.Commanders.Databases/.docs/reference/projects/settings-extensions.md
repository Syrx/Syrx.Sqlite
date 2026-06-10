# Syrx.Commanders.Databases.Settings.Extensions

## Summary
Fluent builder package for constructing `CommanderSettings` and nested command/connection configuration in code.

## Key Types and Interfaces
- `CommanderSettingsBuilder` and `CommanderSettingsBuilderExtensions.Build(...)`.
- `NamespaceSettingBuilder` and extension builder.
- `TypeSettingBuilder<TType>` and extension builder.
- `CommandSettingBuilder` and extension builder.
- `ConnectionStringSettingsBuilder` and extension builder.

## Usage Notes
- Typical build flow:
  - `CommanderSettingsBuilder.AddConnectionString(...)`
  - `CommanderSettingsBuilder.AddCommand(...)`
  - `NamespaceSettingBuilder.ForType<TType>(...)`
  - `TypeSettingBuilder<TType>.ForMethod(...)`
  - `CommandSettingBuilder.UseCommandText(...).UseConnectionAlias(...)`
- Builder guards enforce required values before build completion.
- `CommanderSettingsBuilder` merges namespace/type command entries when duplicate namespaces/types are added.
- `ConnectionStringSettingsBuilder.UseConnectionString(...)` has overloads for alias + value or type-derived alias.

## Thread Safety
- Some builders use `ConcurrentDictionary` internally (`CommanderSettingsBuilder`, `NamespaceSettingBuilder`, `TypeSettingBuilder<TType>`).
- Thread-safety is not guaranteed end-to-end for all mutation paths and merge logic; prefer single-threaded startup construction.

## Security Considerations
- Builder validates shape, not SQL safety.
- Ensure command text comes from trusted sources and stays parameterized.
- Avoid inlining secrets in source; inject connection strings from secure configuration providers.

## Performance Notes
- Designed for startup-time assembly of settings, not per-request execution.
- Dictionary-backed merges reduce repeated linear scans for command aggregation.

## See Also
- [Syrx.Commanders.Databases.Settings](./settings.md)
- [Syrx.Commanders.Databases.Settings.Extensions.Json](./settings-extensions-json.md)
- [Syrx.Commanders.Databases.Settings.Extensions.Xml](./settings-extensions-xml.md)
