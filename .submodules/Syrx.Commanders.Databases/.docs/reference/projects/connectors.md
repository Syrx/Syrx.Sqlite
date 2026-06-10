# Syrx.Commanders.Databases.Connectors

## Summary
Connection abstraction package that maps command connection aliases to concrete provider connections.

## Key Types and Interfaces
- `IDatabaseConnector`: connector contract (`IConnector<IDbConnection, CommandSetting>`).
- `DatabaseConnector`: default implementation using `ICommanderSettings` and `Func<DbProviderFactory>`.
- `ConnectionStringSetting`: alias-to-connection-string configuration model from settings package.

## Usage Notes
- `DatabaseConnector` constructor indexes configured connections by alias and throws on duplicate alias.
- `CreateConnection(CommandSetting setting)`:
  - resolves `setting.ConnectionAlias`;
  - creates a provider connection via factory;
  - applies the resolved connection string;
  - returns the connection (caller opens/uses/disposes it).
- Missing alias or failed provider connection creation raises guarded exceptions.

## Thread Safety
- Alias lookup cache uses `ConcurrentDictionary<string, ConnectionStringSetting>`.
- Initial alias map is built once in constructor and then read-only.
- Connector instance is safe for concurrent `CreateConnection` calls.

## Security Considerations
- Secrets remain in connection strings supplied by configuration; package does not redact by itself.
- Alias-only selection reduces accidental direct string handling in application code.
- Ensure provider factory is trusted and sourced from controlled registration code.

## Performance Notes
- Alias-to-setting cache avoids repeated enumeration of settings collections.
- Factory delegate invocation is cheap and per-call; connection pooling is delegated to provider/Ado.NET.

## See Also
- [Syrx.Commanders.Databases.Settings](./settings.md)
- [Syrx.Commanders.Databases.Connectors.Extensions](./connectors-extensions.md)
- [Connection Management](../architecture/connection-management.md)
