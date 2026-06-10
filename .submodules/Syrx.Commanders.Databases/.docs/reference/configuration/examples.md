# Configuration Examples

Practical configuration examples for JSON, XML, and fluent builder usage.

---

## Summary

All supported configuration inputs converge to `CommanderSettings` and are consumed by the same command resolution and execution pipeline. Preferred guidance is to compose settings with the fluent builders in `Syrx.Commanders.Databases.Settings.Extensions`; JSON/XML loaders remain useful for file-authored settings.

Core settings and runtime consumers:
- [../../../src/Syrx.Commanders.Databases.Settings/CommanderSettings.cs](../../../src/Syrx.Commanders.Databases.Settings/CommanderSettings.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs](../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs)
- [../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs](../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs)

---

## Related Types and APIs

- Fluent builder root: [../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs)
- JSON file registration: [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Json/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Json/UseFileExtensions.cs)
- XML file registration: [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs)
- Command model defaults: [../../../src/Syrx.Commanders.Databases.Settings/CommandSetting.cs](../../../src/Syrx.Commanders.Databases.Settings/CommandSetting.cs)

---

## Examples

### Example 1: Minimal Fluent Builder Configuration (Recommended)

```csharp
var settings = CommanderSettingsBuilderExtensions.Build(builder => builder
  .AddConnectionString("Default", "Server=localhost;Database=MyDb;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;")
  .AddCommand(ns => ns.ForType<UserRepository>(type => type
    .ForMethod(nameof(UserRepository.RetrieveAsync), cmd => cmd
      .UseCommandText("SELECT Id, Name FROM Users WHERE Id = @id")
      .UseConnectionAlias("Default")))));
```

### Example 2: Minimal JSON Configuration

```json
{
  "Connections": [
    {
      "Alias": "Default",
      "ConnectionString": "Server=localhost;Database=MyDb;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;"
    }
  ],
  "Namespaces": [
    {
      "Namespace": "MyApp.Data",
      "Types": [
        {
          "Name": "MyApp.Data.UserRepository",
          "Commands": {
            "RetrieveAsync": {
              "CommandText": "SELECT Id, Name FROM Users WHERE Id = @id",
              "ConnectionAlias": "Default"
            }
          }
        }
      ]
    }
  ]
}
```

### Example 3: Minimal XML Configuration

```xml
<?xml version="1.0" encoding="utf-8"?>
<CommanderSettings>
  <Namespaces>
    <Namespace>
      <Namespace>MyApp.Data</Namespace>
      <Types>
        <Type>
          <Name>MyApp.Data.UserRepository</Name>
          <Commands>
            <RetrieveAsync>
              <CommandText>SELECT Id, Name FROM Users WHERE Id = @id</CommandText>
              <ConnectionAlias>Default</ConnectionAlias>
            </RetrieveAsync>
          </Commands>
        </Type>
      </Types>
    </Namespace>
  </Namespaces>
  <Connections>
    <Connection>
      <Alias>Default</Alias>
      <ConnectionString>Server=localhost;Database=MyDb;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;</ConnectionString>
    </Connection>
  </Connections>
</CommanderSettings>
```

### Example 4: Register File-Based Settings in DI (Optional)

```csharp
var configurationBuilder = new ConfigurationBuilder();

services.UseSyrx(syrx =>
    syrx.UseFile("syrx.settings.json", configurationBuilder));

services.UseSyrx(syrx =>
    syrx.UseFile("syrx.settings.xml", configurationBuilder));
```

File registration methods are implemented in:
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Json/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Json/UseFileExtensions.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs)

### Example 5: Command with Explicit Timeout, Type, and Isolation

```csharp
var settings = CommanderSettingsBuilderExtensions.Build(builder => builder
    .AddConnectionString("Reporting", "Server=localhost;Database=ReportingDb;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;")
    .AddCommand(ns => ns.ForType<ReportRepository>(type => type
        .ForMethod(nameof(ReportRepository.RunMonthlyAsync), cmd => cmd
            .UseCommandText("sp_RunMonthlyReport")
            .UseConnectionAlias("Reporting")
            .SetCommandType(CommandType.StoredProcedure)
            .SetCommandTimeout(120)
            .SetIsolationLevel(IsolationLevel.ReadCommitted)
            .SetFlags(CommandFlagSetting.Buffered | CommandFlagSetting.NoCache)
            .SplitOn("Id")))));
```

---

## Remarks

- Repository command resolution keys are based on fully-qualified type name plus method name.
- Command settings are indexed at reader construction time and cached at runtime by commander and connector components.
- Execute operations open a transaction using `CommandSetting.IsolationLevel` and commit/rollback in `try`/`catch` flow.

Source:
- [../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs](../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs)
- [../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs](../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs)
- [../../../src/Syrx.Commanders.Databases/DatabaseCommander.Execute.cs](../../../src/Syrx.Commanders.Databases/DatabaseCommander.Execute.cs)
- [../../../src/Syrx.Commanders.Databases/DatabaseCommander.ExecuteAsync.cs](../../../src/Syrx.Commanders.Databases/DatabaseCommander.ExecuteAsync.cs)
- [../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs](../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs)

---

## Thread-Safety

- Runtime caches use `ConcurrentDictionary` in commander and connector paths.
- `TypeSetting.Commands` is a `ConcurrentDictionary<string, CommandSetting>`.
- The command reader uses a dictionary built during construction and then used for lookups.

Source:
- [../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs](../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs)
- [../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs](../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs)
- [../../../src/Syrx.Commanders.Databases.Settings/TypeSetting.cs](../../../src/Syrx.Commanders.Databases.Settings/TypeSetting.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs](../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs)

---

## Security

- JSON and XML file registration methods validate file names and reject path traversal patterns.
- Configuration examples intentionally avoid embedding real credentials.

Source:
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Json/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Json/UseFileExtensions.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs)

---

## Performance

- Prefer building or loading settings once at startup.
- Runtime path benefits from cached command and connection lookups.
- Reader indexing removes repeated hierarchical traversal when resolving commands.

Source:
- [../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs](../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs)
- [../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs](../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs)
- [../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs](../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs)

---

## See Also

- [index.md](index.md)
- [json-schema.md](json-schema.md)
- [xml-schema.md](xml-schema.md)
- [builder-api.md](builder-api.md)
