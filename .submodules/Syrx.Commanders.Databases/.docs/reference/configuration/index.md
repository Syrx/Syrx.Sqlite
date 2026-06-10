# Configuration Reference

Entry point for Syrx.Commanders.Databases configuration formats and APIs.

---

## Summary

Syrx configuration resolves into the `CommanderSettings` model, regardless of source. The recommended source is fluent builder composition from `Syrx.Commanders.Databases.Settings.Extensions`, with JSON/XML loaders as optional file-based alternatives.

Core model and runtime consumers:

- [../../../src/Syrx.Commanders.Databases.Settings/CommanderSettings.cs](../../../src/Syrx.Commanders.Databases.Settings/CommanderSettings.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs](../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs)
- [../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs](../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs)

---

## Related Types and APIs

- Builder root: [../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs)
- JSON settings file registration: [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Json/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Json/UseFileExtensions.cs)
- XML settings file registration: [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs)
- Root settings interface: [../../../src/Syrx.Commanders.Databases.Settings/ICommanderSettings.cs](../../../src/Syrx.Commanders.Databases.Settings/ICommanderSettings.cs)
- Command settings model: [../../../src/Syrx.Commanders.Databases.Settings/CommandSetting.cs](../../../src/Syrx.Commanders.Databases.Settings/CommandSetting.cs)

---

## Examples

Use this section as navigation to full examples and format-specific references:

- Fluent builder API (recommended): [builder-api.md](builder-api.md)
- End-to-end examples: [examples.md](examples.md)
- JSON format: [json-schema.md](json-schema.md)
- XML format: [xml-schema.md](xml-schema.md)

---

## Remarks

- Command resolution depends on type name and method name mapping in the settings reader.
- `CommandSetting` default values are applied in code when properties are omitted in configuration input.
- File-based loaders enforce extension and file-name safety checks before registration.

Supporting source:

- [../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs](../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs)
- [../../../src/Syrx.Commanders.Databases.Settings/CommandSetting.cs](../../../src/Syrx.Commanders.Databases.Settings/CommandSetting.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Json/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Json/UseFileExtensions.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs)

---

## Thread-Safety

- Runtime command and connection lookup paths use `ConcurrentDictionary` caches.
- `TypeSetting.Commands` is defined as `ConcurrentDictionary<string, CommandSetting>`.

Source:

- [../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs](../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs)
- [../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs](../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs)
- [../../../src/Syrx.Commanders.Databases.Settings/TypeSetting.cs](../../../src/Syrx.Commanders.Databases.Settings/TypeSetting.cs)

---

## Security

- JSON and XML loader extensions validate trusted file names by rejecting path separators and enforcing expected extensions.
- This page and linked examples avoid embedding production secrets.

Source:

- [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Json/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Json/UseFileExtensions.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs)

---

## Performance

- The reader builds a command lookup dictionary once in its constructor.
- Commander and connector components cache lookups to avoid repeated traversal and alias resolution costs.

Source:

- [../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs](../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs)
- [../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs](../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs)
- [../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs](../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs)

---

## See Also

- [../index.md](../index.md)
- [json-schema.md](json-schema.md)
- [xml-schema.md](xml-schema.md)
- [builder-api.md](builder-api.md)
- [examples.md](examples.md)