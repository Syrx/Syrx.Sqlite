# XML Schema Reference

XML-based configuration reference for Syrx.Commanders.Databases.

---

## Summary

The XML configuration path loads settings through the .NET configuration pipeline and binds to the same settings model used by other formats. This is an optional file-based alternative to the recommended fluent builders in `Syrx.Commanders.Databases.Settings.Extensions`.

The XML file registration entry point is `UseFile(this SyrxBuilder factory, string fileName, IConfigurationBuilder builder)` in [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs).

The bound target model is `CommanderSettings` in [../../../src/Syrx.Commanders.Databases.Settings/CommanderSettings.cs](../../../src/Syrx.Commanders.Databases.Settings/CommanderSettings.cs), including nested `NamespaceSetting`, `TypeSetting`, `CommandSetting`, and `ConnectionStringSetting` records.

---

## Related Types and APIs

- XML loader extension: [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs)
- Root settings contract: [../../../src/Syrx.Commanders.Databases.Settings/ICommanderSettings.cs](../../../src/Syrx.Commanders.Databases.Settings/ICommanderSettings.cs)
- Root settings record: [../../../src/Syrx.Commanders.Databases.Settings/CommanderSettings.cs](../../../src/Syrx.Commanders.Databases.Settings/CommanderSettings.cs)
- Namespace model: [../../../src/Syrx.Commanders.Databases.Settings/NamespaceSetting.cs](../../../src/Syrx.Commanders.Databases.Settings/NamespaceSetting.cs)
- Type model: [../../../src/Syrx.Commanders.Databases.Settings/TypeSetting.cs](../../../src/Syrx.Commanders.Databases.Settings/TypeSetting.cs)
- Command model: [../../../src/Syrx.Commanders.Databases.Settings/CommandSetting.cs](../../../src/Syrx.Commanders.Databases.Settings/CommandSetting.cs)
- Connection model: [../../../src/Syrx.Commanders.Databases.Settings/ConnectionStringSetting.cs](../../../src/Syrx.Commanders.Databases.Settings/ConnectionStringSetting.cs)

---

## Examples

### Minimal XML Settings

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

### XML with Optional Command Fields

```xml
<?xml version="1.0" encoding="utf-8"?>
<CommanderSettings>
  <Namespaces>
    <Namespace>
      <Namespace>MyApp.Data</Namespace>
      <Types>
        <Type>
          <Name>MyApp.Data.ReportRepository</Name>
          <Commands>
            <RunMonthlyAsync>
              <CommandText>sp_RunMonthlyReport</CommandText>
              <ConnectionAlias>Reporting</ConnectionAlias>
              <CommandType>StoredProcedure</CommandType>
              <CommandTimeout>120</CommandTimeout>
              <IsolationLevel>ReadCommitted</IsolationLevel>
              <Flags>Buffered, NoCache</Flags>
              <Split>Id</Split>
            </RunMonthlyAsync>
          </Commands>
        </Type>
      </Types>
    </Namespace>
  </Namespaces>
  <Connections>
    <Connection>
      <Alias>Reporting</Alias>
      <ConnectionString>Server=localhost;Database=ReportingDb;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;</ConnectionString>
    </Connection>
  </Connections>
</CommanderSettings>
```

### Register XML Configuration

```csharp
var configBuilder = new ConfigurationBuilder();

services.UseSyrx(builder =>
    builder.UseFile("syrx.settings.xml", configBuilder));

var configuration = configBuilder.Build();
var settings = configuration.Get<CommanderSettings>();
```

The `UseFile` method for XML registration is implemented in [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs).

---

## Remarks

- XML and JSON both converge to `CommanderSettings`; execution code consumes `CommandSetting` regardless of source format.
- Required values in the model are `CommandText`, `ConnectionAlias`, and naming keys used in `Namespaces`, `Types`, and `Commands`.
- `CommandSetting` defaults are set in code: `Split = "Id"`, `CommandTimeout = 30`, `CommandType = CommandType.Text`, `Flags = Buffered | NoCache`, `IsolationLevel = Serializable`.

Defaults are defined in [../../../src/Syrx.Commanders.Databases.Settings/CommandSetting.cs](../../../src/Syrx.Commanders.Databases.Settings/CommandSetting.cs).

---

## Thread-Safety

- The XML loader extension itself is stateless and performs input validation before calling `AddXmlFile`.
- After binding, `TypeSetting.Commands` uses `ConcurrentDictionary<string, CommandSetting>`, which is used throughout command resolution and caching paths.

Relevant source:
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs)
- [../../../src/Syrx.Commanders.Databases.Settings/TypeSetting.cs](../../../src/Syrx.Commanders.Databases.Settings/TypeSetting.cs)

---

## Security

- XML file registration explicitly rejects file names containing directory separators and requires a `.xml` extension.
- This reduces path traversal risk when registering settings files.

Validation logic is in [../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions.Xml/UseFileExtensions.cs).

---

## Performance

- XML parsing cost is paid at configuration load time; runtime command execution does not parse XML.
- Runtime lookups are served through caches in commander, connector, and command-reader layers.

Caching implementations:
- Command cache: [../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs](../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs)
- Connection alias cache: [../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs](../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs)
- Command lookup index: [../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs](../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs)

---

## See Also

- [index.md](index.md)
- [builder-api.md](builder-api.md)
- [json-schema.md](json-schema.md)
- [examples.md](examples.md)
