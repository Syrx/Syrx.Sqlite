# Builder API Reference

Fluent, programmatic configuration reference for Syrx.Commanders.Databases.

---

## Summary

The builder API composes `CommanderSettings` in memory using strongly-typed fluent builders.

Entry points and builders are implemented in:
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilderExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilderExtensions.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/NamespaceSettingBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/NamespaceSettingBuilder.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/TypeSettingBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/TypeSettingBuilder.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommandSettingBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommandSettingBuilder.cs)

---

## Related Types and APIs

### Root Composition

- `CommanderSettingsBuilder.AddConnectionString(string alias, string connectionString)`
- `CommanderSettingsBuilder.AddConnectionString(Action<ConnectionStringSettingsBuilder> builder)`
- `CommanderSettingsBuilder.AddCommand(Action<NamespaceSettingBuilder> builder)`
- `CommanderSettingsBuilderExtensions.Build(Action<CommanderSettingsBuilder> factory)`

Source:
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilderExtensions.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilderExtensions.cs)

### Namespace and Type Composition

- `NamespaceSettingBuilder.ForType<TType>(Action<TypeSettingBuilder<TType>> builder)`
- `TypeSettingBuilder<TType>.ForMethod(string method, Action<CommandSettingBuilder> builder)`

Source:
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/NamespaceSettingBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/NamespaceSettingBuilder.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/TypeSettingBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/TypeSettingBuilder.cs)

### Command Composition

- `CommandSettingBuilder.UseCommandText(string commandText)`
- `CommandSettingBuilder.UseConnectionAlias(string alias)`
- `CommandSettingBuilder.SetCommandTimeout(int commandTimeout = 30)`
- `CommandSettingBuilder.SetCommandType(CommandType commandType = CommandType.Text)`
- `CommandSettingBuilder.SetFlags(CommandFlagSetting commandFlagSetting = Buffered | NoCache)`
- `CommandSettingBuilder.SetIsolationLevel(IsolationLevel isolationLevel = Serializable)`
- `CommandSettingBuilder.SplitOn(string split = "id")`

Source:
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommandSettingBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommandSettingBuilder.cs)

---

## Examples

### Build Minimal Settings

```csharp
var settings = CommanderSettingsBuilderExtensions.Build(builder => builder
    .AddConnectionString("Default", "Server=localhost;Database=MyDb;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;")
    .AddCommand(ns => ns.ForType<UserRepository>(type => type
        .ForMethod(nameof(UserRepository.RetrieveAsync), cmd => cmd
            .UseCommandText("SELECT Id, Name FROM Users WHERE Id = @id")
            .UseConnectionAlias("Default")))));
```

### Build with Explicit Command Options

```csharp
var settings = CommanderSettingsBuilderExtensions.Build(builder => builder
    .AddConnectionString(cs => cs
        .UseAlias("Reporting")
        .UseConnectionString("Server=localhost;Database=ReportingDb;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;"))
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

### Merge Additional Namespace Settings

```csharp
var ns = NamespaceSettingBuilderExtensions.Build(n => n
    .ForType<UserRepository>(t => t
        .ForMethod("RetrieveAsync", c => c
            .UseCommandText("SELECT Id FROM Users WHERE Id = @id")
            .UseConnectionAlias("Default"))));

var settings = CommanderSettingsBuilderExtensions.Build(root => root
    .AddConnectionString("Default", "Server=localhost;Database=MyDb;Integrated Security=true;Encrypt=true;TrustServerCertificate=false;")
    .AddCommand(ns));
```

---

## Remarks

- The builder requires at least one namespace command entry before `Build()` succeeds.
- `SetCommandTimeout` rejects values less than or equal to 1.
- Duplicate connection aliases are allowed only when the connection string value is identical.
- Command defaults are inherited from `CommandSettingBuilder` and materialized into `CommandSetting`.

Behavior source:
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommandSettingBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommandSettingBuilder.cs)

---

## Thread-Safety

- Builder internals use `ConcurrentDictionary` for command and connection aggregation.
- Builder instances are mutable objects; create and compose each instance in a controlled scope.

Source:
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/NamespaceSettingBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/NamespaceSettingBuilder.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/TypeSettingBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/TypeSettingBuilder.cs)

---

## Security

- Builder methods validate required values and fail fast via guard checks (`Throw<...>`), reducing accidental invalid configuration states.
- Connection strings are plain strings in settings; secret storage decisions remain the caller's responsibility.

Source:
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/CommanderSettingsBuilder.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Extensions/ConnectionStringSettingsBuilder.cs](../../../src/Syrx.Commanders.Databases.Settings.Extensions/ConnectionStringSettingsBuilder.cs)

---

## Performance

- Builder composition happens once during startup or initialization in typical usage.
- The produced settings feed runtime components that cache command and connection lookups.

Runtime cache sources:
- [../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs](../../../src/Syrx.Commanders.Databases/DatabaseCommander.cs)
- [../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs](../../../src/Syrx.Commanders.Databases.Connectors/DatabaseConnector.cs)
- [../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs](../../../src/Syrx.Commanders.Databases.Settings.Readers/DatabaseCommandReader.cs)

---

## See Also

- [index.md](index.md)
- [json-schema.md](json-schema.md)
- [xml-schema.md](xml-schema.md)
- [examples.md](examples.md)
