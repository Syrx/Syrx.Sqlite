# Syrx.Commanders.Databases.Settings

## Summary
Configuration model package that defines how repository methods map to executable database commands and connection aliases.

## Key Types and Interfaces
- `ICommanderSettings`: top-level settings contract.
- `CommanderSettings`: concrete settings record.
- `NamespaceSetting`: namespace-scoped repository grouping.
- `TypeSetting`: repository type mapping with command dictionary.
- `CommandSetting`: command metadata (`CommandText`, timeout, type, flags, isolation, split).
- `ConnectionStringSetting`: alias and connection string pair.
- `CommandFlagSetting`: Dapper command flag enum (`Buffered`, `Pipelined`, `NoCache`).

## Usage Notes
- Commands are resolved by repository type full name + method name.
- `CommandSetting.ConnectionAlias` links each command to a named connection entry.
- Defaults in `CommandSetting`:
  - `CommandTimeout = 30`
  - `CommandType = Text`
  - `Flags = Buffered | NoCache`
  - `IsolationLevel = Serializable`
  - `Split = "Id"`
- `TypeSetting.Commands` is a `ConcurrentDictionary<string, CommandSetting>` for command key lookups.

## Thread Safety
- Settings records use init-only properties, but many properties are mutable collection types (`List<>`, `ConcurrentDictionary<>`).
- Treat deserialized/built settings as read-only after startup.
- `TypeSetting.Commands` supports concurrent reads/writes technically; design intent is runtime read-mostly usage.

## Security Considerations
- Contains sensitive connection string material; load from secure config sources.
- SQL command text is trusted configuration input; ensure parameterized SQL and restricted edit paths.
- Avoid exposing full settings objects in logs.

## Performance Notes
- Compact data-only records with low overhead.
- `ConcurrentDictionary` in `TypeSetting` supports efficient command access for readers.
- Startup cost is proportional to command count; runtime lookups are dictionary-based.

## See Also
- [Syrx.Commanders.Databases.Settings.Extensions](./settings-extensions.md)
- [Syrx.Commanders.Databases.Settings.Readers](./settings-readers.md)
- [Configuration Overview](../configuration/index.md)
